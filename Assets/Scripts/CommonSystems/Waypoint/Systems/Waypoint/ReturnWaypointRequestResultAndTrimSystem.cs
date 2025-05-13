using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(UpdateWaypointSystemGroup))]
[UpdateAfter(typeof(CompleteWaypointRequestSystem))]
public partial struct ReturnWaypointRequestResultAndTrimSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<CachedPathData>();
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            PathIsHandling
          , Simulate>().Build());
    }

    public void OnUpdate(ref SystemState state) {
        var curTick    = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
        var cachedPath = SystemAPI.ManagedAPI.GetSingleton<CachedPathData>();

        foreach (var (
            waypoints
          , handlingData
          , handlingTrigger
            , moveData
            ) in SystemAPI
            .Query<
                DynamicBuffer<WaypointBuffer>
              , RefRW<PathHandlingData>
              , EnabledRefRW<PathIsHandling>
            , RefRW<MoveData>>()
            .WithAll<Simulate>()) {
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            if (handlingData.ValueRO.doneAtTick.IsNewerThan(curTick))
                continue;

            handlingTrigger.ValueRW = false;

            waypoints.Clear();
            foreach (var point in cachedPath.GetData(handlingData.ValueRO.newPID.code).waypoints)
                waypoints.Add(new WaypointBuffer(point.Quantizate3()));

            // This is very necessary
            moveData.ValueRW.isMoveDone = false;
        }
    }
}