using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(PrepareMoveSystemGroup))]
public partial struct GetMoveDataFrom_StatsMoveSpeedSystem : ISystem {
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
        public void Execute(ref MoveData moveData, in DynamicBuffer<StatsBuffer> stats) {
            moveData.moveSpeed = stats[StatsId.MoveSpeed].value;
        }
    }
}