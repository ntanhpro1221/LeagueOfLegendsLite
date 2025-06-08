using Pathfinding.ECS;
using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(BeforeFollowerEntityCalculateSystemGroup))]
public partial struct GetFollowerSpeed_FromStatsSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref MovementSettings moveData, in DynamicBuffer<StatsBuffer> stats) {
            moveData.follower.speed = stats[StatsId.MoveSpeed].value;
        }
    }
}