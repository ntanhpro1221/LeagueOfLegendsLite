using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
public partial struct InitDynamicHealthSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                Simulate
              , StatsBuffer
            >().WithDisabled<
                HealthData
            >().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(StatsBuffer))]
    [WithDisabled(
        typeof(HealthData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  DynamicBuffer<StatsBuffer> stats
          , ref HealthData                 health
          , EnabledRefRW<HealthData>       healthTrigger) {
            health.value          = stats[(int)StatsType.Health].value;
            healthTrigger.ValueRW = true;
        }
    }
}