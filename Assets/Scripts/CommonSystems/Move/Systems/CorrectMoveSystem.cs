using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial struct CorrectMoveSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(in MoveData moveData, ref LocalTransform localTrans, ref PhysicsVelocity velocity) {
            if (!moveData.isMoveDone) return;

            localTrans.Position.AssignKeepY(moveData.targetLocPos.Full);
            velocity.Linear.AssignKeepY(float3.zero);
        }
    }
}