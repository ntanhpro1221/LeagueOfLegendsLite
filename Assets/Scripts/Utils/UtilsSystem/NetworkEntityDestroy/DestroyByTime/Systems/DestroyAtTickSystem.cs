using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(BeforeDestroyNetworkEntitySystemGroup))]
public partial struct DestroyAtTickSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();

        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                DestroyAtTick
              , Simulate>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var netTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!netTime.IsFirstTimeFullyPredictingTick) return;

        state.Dependency = new Job {
            curTick = netTime.ServerTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [WithPresent(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public NetworkTick curTick;

        [BurstCompile]
        public void Execute(
            ref DestroyAtTick                 destroyTick
          , EnabledRefRW<DestroyAtTick>       destroyTickEnable
          , EnabledRefRW<NetworkDestroyedTag> networkDestroyEnable) {
            if (destroyTick.tick.IsNewerThan(curTick))
                return;
            destroyTickEnable.ValueRW    = false;
            networkDestroyEnable.ValueRW = true;
        }
    }
}