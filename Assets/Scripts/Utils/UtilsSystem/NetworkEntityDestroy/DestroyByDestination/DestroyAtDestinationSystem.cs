using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(BeforeDestroyNetworkEntitySystemGroup))]
public partial struct DestroyAtDestinationSystem : ISystem {
    private const float DESTINATION_TOLERANCE_SQR = 1f;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();

        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                DestroyAtDestination
              , LocalToWorld
              , Simulate>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [WithPresent(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(
            ref DestroyAtDestination           destroyDes
          , EnabledRefRW<DestroyAtDestination> destroyDesEnable
          , in DestroyAtDesSettings            settings
          , EnabledRefRW<NetworkDestroyedTag>  networkDestroyEnable
          , in LocalTransform                  locTrans) {
            switch (settings.useY) {
                case true when DESTINATION_TOLERANCE_SQR
                  < math.distancesq(locTrans.Position, destroyDes.destination):
                case false when DESTINATION_TOLERANCE_SQR
                  < GameHelpers.DistanceXZ_Sqr(locTrans.Position, destroyDes.destination):
                    return;
            }

            destroyDesEnable.ValueRW     = false;
            networkDestroyEnable.ValueRW = true;
        }
    }
}