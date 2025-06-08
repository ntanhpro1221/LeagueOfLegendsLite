using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(UpdateWaypointSystemGroup))]
[UpdateAfter(typeof(HandleWaypointRequestSystem))]
public partial struct CompleteWaypointRequestSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<CachedPathData>();
        state.RequireForUpdate<HandlingPathData>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                Simulate
              , PathIsHandling
            >().Build();
    }

    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;
        
        var curTick    = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
        var cachedPath = SystemAPI.ManagedAPI.GetSingleton<CachedPathData>();

        SystemAPI.ManagedAPI.GetSingleton<HandlingPathData>().ForceComplete(curTick, cachedPath);

        // TODO: Consider funnel modifier, it may be better according to the document
        // Use Raycast modifier at this moment
        var modifier = PathModifierHub.Raycast;
        foreach (var (
            handlingData
          , locTrans
            ) in SystemAPI
            .Query<
                RefRW<PathHandlingData>
              , RefRO<LocalTransform>>()
            .WithAll<
                Simulate
              , PathIsHandling>()) {
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            if (handlingData.ValueRO.doneAtTick.IsNewerThan(curTick))
                continue;

            int newPathCode = handlingData.ValueRO.newPID.code;

            // If there is a new pid that has not done yet
            // The only reason is we not sync from org pid to new pid
            // (we just call pathfinding on org pid)
            if (!cachedPath.ContainsCode(newPathCode))
                HandleWaypointRequestSystem.CachePathDirectly(
                    cachedPath
                  , handlingData.ValueRO.newPID
                  , handlingData.ValueRO.doneAtTick
                  , cachedPath.GetData(handlingData.ValueRO.orgPID.code).waypoints
                  , modifier);
            ref var cachedData = ref cachedPath.GetData(newPathCode);

            var myQuantizedPos = locTrans.ValueRO.Position.Quantizate3();
            if (cachedData.isAppliedModify
             && 1 > GameHelpers.DistanceXZ_Sqr(myQuantizedPos
                  , cachedData.originPnt))
                continue;

            cachedData.waypoints.Add(myQuantizedPos);
            cachedData.ApplyModify(modifier);
        }
    }
}