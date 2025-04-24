using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(BeforeDestroyNetworkEntitySystemGroup))]
[UpdateBefore(typeof(DestroyAtTickSystem))]
public partial struct InitDestroyAtTickSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<ClientServerTickRate>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                DestroyAfterPeriod
              , Simulate>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var netTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!netTime.IsFirstTimeFullyPredictingTick) return;

        state.Dependency = new Job {
            curTick  = netTime.ServerTick
          , tickRate = SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [WithPresent(typeof(DestroyAtTick))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public NetworkTick curTick;
        public int         tickRate;

        [BurstCompile]
        public void Execute(
            ref DestroyAfterPeriod           destroyPeriod
          , EnabledRefRW<DestroyAfterPeriod> destroyPeriodEnable
          , ref DestroyAtTick                destroyTick
          , EnabledRefRW<DestroyAtTick>      destroyTickEnable) {
            destroyPeriodEnable.ValueRW = false;

            destroyTick.tick          = curTick.WithDeltaTime(destroyPeriod.lifeTime, tickRate);
            destroyTickEnable.ValueRW = true;
        }
    }
}