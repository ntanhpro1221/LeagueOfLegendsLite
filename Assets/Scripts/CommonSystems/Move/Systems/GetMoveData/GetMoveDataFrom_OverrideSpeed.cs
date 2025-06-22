using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(PrepareMoveSystemGroup), OrderLast = true)]
public partial struct GetMoveDataFrom_OverrideSpeed : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MoveSpeedOverride))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref MoveData moveData, in MoveSpeedOverrideData speed) {
            moveData.moveSpeed = speed.speed;
        }
    }
}