using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup), OrderLast = true)]
public partial struct DisableAllInitRequestSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                Simulate
              , NeedInitTag
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
      , typeof(NeedInitTag))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(EnabledRefRW<NeedInitTag> initTrigger) {
            initTrigger.ValueRW = false;
        }
    }
}