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
              , StatsBuffer
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
      , typeof(StatsBuffer))]
    [WithDisabled(
        typeof(ManaData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  DynamicBuffer<StatsBuffer> stats
          , ref ManaData                   mana
          , EnabledRefRW<ManaData>         manaTrigger) {
            mana.value          = stats[(int)StatsType.Mana].value;
            manaTrigger.ValueRW = true;
        }
    }
}