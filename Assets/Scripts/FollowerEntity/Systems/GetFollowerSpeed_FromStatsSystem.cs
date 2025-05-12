using Pathfinding.ECS;
using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(BeforeFollowerEntityCalculateSystemGroup))]
public partial struct GetFollowerSpeed_FromStatsSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            moveSpeedId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.MoveSpeed]
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        public int moveSpeedId;

        [BurstCompile]
        public void Execute(ref MovementSettings moveData, in DynamicBuffer<StatsBuffer> stats) {
            moveData.follower.speed = stats[moveSpeedId].value;
        }
    }
}