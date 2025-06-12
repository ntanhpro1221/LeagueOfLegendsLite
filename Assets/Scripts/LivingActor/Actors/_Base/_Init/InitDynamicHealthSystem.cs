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
              , StatsData
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
      , typeof(StatsData))]
    [WithDisabled(
        typeof(HealthData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  StatsData            stats
          , ref HealthData           health
          , in  Entity               entity
          , EnabledRefRW<HealthData> healthTrigger) {
            health.value          = stats.data.Health;
            healthTrigger.ValueRW = true;
        }
    }
}