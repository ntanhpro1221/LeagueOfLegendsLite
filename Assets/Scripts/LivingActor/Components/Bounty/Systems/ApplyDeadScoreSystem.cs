using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleBountySystemGroup))]
public partial struct ApplyDeadScoreSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                Simulate
              , BountyTrigger
            >().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        state.Dependency = new Job()
            .Schedule(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(BountyTrigger))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref KDAData kda) {
            kda.dead++;
        }
    }
}