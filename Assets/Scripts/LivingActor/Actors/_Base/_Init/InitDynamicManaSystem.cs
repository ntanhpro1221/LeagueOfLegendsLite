using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
public partial struct InitDynamicManaSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                Simulate
              , StatsData
            >().WithDisabled<
                ManaData
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
        typeof(ManaData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  StatsData          stats
          , ref ManaData           mana
          , EnabledRefRW<ManaData> manaTrigger) {
            mana.value          = stats.data.Mana;
            manaTrigger.ValueRW = true;
        }
    }
}