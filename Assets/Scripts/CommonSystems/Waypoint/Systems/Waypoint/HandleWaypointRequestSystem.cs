using System.Collections.Generic;
using NGDtuanh.UnsafePooling;
using Pathfinding;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(UpdateWaypointSystemGroup))]
public partial struct HandleWaypointRequestSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<WaypointCalculateConfig>();
        state.RequireForUpdate<CachedPathData>();
        state.RequireForUpdate<CachedLineCastData>();
        state.RequireForUpdate<HandlingPathData>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<Simulate>()
            .WithAny<
                PathIsHandling
              , NeedHandleWaypointRequest>()
            .Build());
    }

    public void OnUpdate(ref SystemState state) {
        var  cachedPath        = SystemAPI.ManagedAPI.GetSingleton<CachedPathData>();
        var  cachedLinecast    = SystemAPI.ManagedAPI.GetSingleton<CachedLineCastData>();
        var  handlingPath      = SystemAPI.ManagedAPI.GetSingleton<HandlingPathData>();
        bool isClient          = state.WorldUnmanaged.IsClient();
        var  champGraph         = AstarPath.active.graphs[GraphIndexHelper.Champion] as NavmeshBase;
        var  modifier          = PathModifierHub.Raycast;
        var  config            = SystemAPI.GetSingleton<WaypointCalculateConfig>();
        var  fixablePathDisSqr = config.fixablePathDisSqr;
        var  curTick           = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
        var  curDoneAtTick     = config.DoneAtTick(curTick);

        if (champGraph == null) {
            Debug.LogError("Fail to get champion graph");
            return;
        }
        
        foreach (var (
            rawHandlingData
          , rawRequestData
          , handlingTrigger
          , requestTrigger
          , waypoints) in SystemAPI
            .Query<
                RefRW<PathHandlingData>
              , RefRO<WaypointRequestData>
              , EnabledRefRW<PathIsHandling>
              , EnabledRefRW<NeedHandleWaypointRequest>
              , DynamicBuffer<WaypointBuffer>>()
            .WithAny<
                PathIsHandling
              , NeedHandleWaypointRequest>()
            .WithAll<Simulate>()) {
            
            var orgHandlingTrigger = handlingTrigger.ValueRO;
            var orgRequestTrigger  = requestTrigger.ValueRO;

            // First: Mark request done anyway
            requestTrigger.ValueRW  = false;
            handlingTrigger.ValueRW = true;

            ref var handlingData = ref rawHandlingData.ValueRW;
            var     requestData  = rawRequestData.ValueRO;

            // New request when path is handling
            // And new pid can see ORIGIN pid
            // ==> Update it's new PID and CONTINUE.
            if (orgHandlingTrigger && orgRequestTrigger
             && !cachedLinecast.Linecast(curTick, champGraph, new PathId(
                        handlingData.orgPID.start
                      , requestData.pid.start))
                    .haveObstacle
             && !cachedLinecast.Linecast(curTick, champGraph, new PathId(
                        handlingData.orgPID.end
                      , requestData.pid.end))
                    .haveObstacle) {
                // Update new pid
                handlingData.UpdatePID(requestData.pid);

                // We have to set temporary waypoint to fake move when waiting for calculating done
                AssignTemporaryWaypoint(
                    waypoints
                  , requestData.pid
                  , cachedLinecast
                  , curTick
                  , champGraph);
                continue;
            }

            // When this is unexpected handling path (mostly from roll back and pathCode change)
            // ==> Yes do nothing instead of continue because this is unexpected, it is not exists in cache
            // (We will filter out if this is already in cache below)
            if (orgHandlingTrigger && !orgRequestTrigger) { }
            // Otherwise, just set completely new handling data
            else
                handlingData = new(
                    curDoneAtTick
                  , requestData.pid.start
                  , requestData.pid.end);

            var doneAtTick = handlingData.doneAtTick;
            var orgPID     = handlingData.orgPID;
            var newPID     = handlingData.newPID;

            if (TryFilterOutCachedPath(orgPID
              , cachedPath, ref handlingData, curTick, ref doneAtTick))
                continue;

            if (orgPID.code != newPID.code
             && TryFilterOutCachedPath(newPID
                  , cachedPath, ref handlingData, curTick, ref doneAtTick))
                continue;

            if (TryFilterOutHandlingPath(orgPID
              , handlingPath, cachedLinecast, curTick, doneAtTick, waypoints, orgRequestTrigger, champGraph))
                continue;
            
            if (orgPID.code != newPID.code
            && TryFilterOutHandlingPath(newPID
              , handlingPath, cachedLinecast, curTick, doneAtTick, waypoints, orgRequestTrigger, champGraph))
                continue;
            
            // There is no obstacle, so just go ahead
            if (!cachedLinecast.Linecast(curTick, champGraph, orgPID).haveObstacle) {
                // Cache origin path
                CachePathDirectly(
                    cachedPath
                  , orgPID
                  , curTick); // because return result immediately
                ref var cachedOrgData = ref cachedPath.GetData(orgPID.code);
                cachedOrgData.isAppliedModify      = true;
                cachedOrgData.canReturnImmediately = true;

                // Cache new path
                if (orgPID.code != newPID.code)  {
                    CachePathDirectly(
                        cachedPath
                      , newPID
                      , curTick // because return result immediately
                      , cachedPath.GetData(orgPID.code).waypoints
                      , modifier);
                    if (!cachedLinecast.Linecast(curTick, champGraph, newPID).haveObstacle)
                        cachedPath.GetData(newPID.code).canReturnImmediately = true;
                }

                // try return immediately
                if (cachedPath.GetData(newPID.code).canReturnImmediately)
                    handlingData.doneAtTick = curTick;

                continue;
            }

            // There is request when not handling path and already have waypoint
            // Try to fix existing waypoint
            if (!orgHandlingTrigger && !waypoints.IsEmpty) {
                float3 newTarget        = newPID.end;
                int    nearestOldEndPnt = -1;

                for (int i = waypoints.Length - 1; i >= 0; --i) {
                    // Too far, we skip it to save our CPU resource
                    if (fixablePathDisSqr < GameHelpers.DistanceXZ_Sqr(waypoints[i].pos, newTarget))
                        continue;

                    // There is obstacle, skip
                    if (cachedLinecast.Linecast(curTick, champGraph, new PathId(
                            waypoints[i].pos
                          , newTarget.Quantizate3()))
                        .haveObstacle)
                        continue;

                    nearestOldEndPnt = i;
                    break;
                }

                if (nearestOldEndPnt != -1) {
                    // Pool list waypoint and set value
                    var prevPath = ListPool<Vector3>.Claim();
                    for (int j = nearestOldEndPnt; j < waypoints.Length; ++j)
                        prevPath.Add(waypoints[j].pos);

                    // Note that just this time, we need to return this path immediately

                    // Cache with curTick without modifier
                    CachePathDirectly(
                        cachedPath
                      , newPID
                      , curTick
                      , prevPath);
                    cachedPath.GetData(newPID.code).isAppliedModify = true;

                    // Return immediately
                    handlingData.doneAtTick = curTick;

                    // Release list to pool
                    ListPool<Vector3>.Release(prevPath);

                    continue;
                }
            }

            // We have to set temporary waypoint to fake move when waiting for calculating done
            AssignTemporaryWaypoint(
                waypoints
              , newPID
              , cachedLinecast
              , curTick
              , champGraph);

            // If this path has not in handling state yet
            if (!handlingPath.ContainsCode(orgPID.code)) {
                // Create new path request
                var path = ABPath.Construct(
                    CachedLineCastData.TryGetExactlyEdgePnt(orgPID.start, champGraph, out _)
                  , CachedLineCastData.TryGetExactlyEdgePnt(orgPID.end,   champGraph, out _));
                PathHolderForWaypoint.Claim(path);
                
                path.nnConstraint.distanceMetric = DistanceMetric.ClosestAsSeenFromAbove(math.up());
                path.nnConstraint.graphMask      = GraphMask.FromGraphIndex(GraphIndexHelper.Champion);

                // Save handling path
                handlingPath.PushData(orgPID.code, HandlingPathData.NewData(path));

                // Push request to Astar
                if (isClient)
                    AstarPath.StartPath(path);        // When run in CLIENT: normal push for stable calculation
                else AstarPath.StartPath(path, true); // When run in SERVER: push to front to calculate before minion
            }

            handlingPath.PushTick(orgPID.code, doneAtTick);
        }
    }

    public static bool TryFilterOutCachedPath(
        in  PathId           pid
      , in  CachedPathData   cachedPath
      , ref PathHandlingData handlingData
      , in  NetworkTick      curTick
      , ref NetworkTick      doneAtTick) {
        if (!cachedPath.ContainsCode(pid.code)) return false;
        
        if (cachedPath.IsCanReturnImmediately(pid.code))
            doneAtTick = handlingData.doneAtTick = curTick;

        if (!cachedPath.ContainsTick(pid.code, doneAtTick))
            cachedPath.PushTick(pid.code, doneAtTick);
            
        return true;
    }

    public static bool TryFilterOutHandlingPath(
        in  PathId                        pid
      , in  HandlingPathData              handlingPath
      , in  CachedLineCastData            cachedLinecast
      , in  NetworkTick                   curTick
      , in NetworkTick                   doneAtTick
      , in  DynamicBuffer<WaypointBuffer> waypoints
      , bool                              orgRequestTrigger
      , in NavmeshBase                    champGraph) {
        if (!handlingPath.ContainsCode(pid.code)) return false;

        if (!handlingPath.ContainsTick(pid.code, doneAtTick))
            handlingPath.PushTick(pid.code, doneAtTick);

        // We have to set temporary waypoint to fake move when waiting for calculating done
        if (orgRequestTrigger)
            AssignTemporaryWaypoint(
                waypoints
              , pid
              , cachedLinecast
              , curTick
              , champGraph);

        return true;
    }

    public static void AssignTemporaryWaypoint(
        in DynamicBuffer<WaypointBuffer> waypoints
      , PathId                           pid
      , CachedLineCastData               cachedLinecast
      , NetworkTick                      curTick
      , NavmeshBase                      graph) {
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        waypoints.Clear();
        
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        waypoints.Add(new WaypointBuffer(
            cachedLinecast.Linecast(curTick, graph, pid)
                .furthestPnt
                .Quantizate3()));
    }

    public static void CachePathDirectly(
        CachedPathData cachedPath
      , PathId         pid
      , NetworkTick    doneAtTick
      , List<Vector3>  middle   = null
      , MonoModifier   modifier = null) {
        if (cachedPath.ContainsTick(pid.code, doneAtTick))
            return;

        if (!cachedPath.ContainsCode(pid.code)) {
            // init path
            var path = ABPath.Construct(pid.start, pid.end);
            PathHolderForWaypoint.Claim(path);

            // add points to path
            path.vectorPath.Add(path.endPoint);
            if (middle != null)
                foreach (var pnt in middle)
                    path.vectorPath.Add(pnt);
            path.vectorPath.Add(path.startPoint);

            // cache path
            ref var cachedData = ref cachedPath.PushData(pid.code, CachedPathData.NewData(path));

            // apply modifier
            if (modifier != null)
                cachedData.ApplyModify(modifier);
        }

        cachedPath.PushTick(pid.code, doneAtTick);
    }
}