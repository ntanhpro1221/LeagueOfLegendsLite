using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(MoveSystemGroup))]
public partial struct ApplyMoveSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<CommonGameRulesData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new ApplyMoveJob {
            rotateSpeed = SystemAPI.GetSingleton<CommonGameRulesData>().rotateSpeed
          , deltaTime   = SystemAPI.Time.DeltaTime
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new StopDisabledMoveJob()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MoveableTag))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    public partial struct ApplyMoveJob : IJobEntity {
        public float rotateSpeed;
        public float deltaTime;

        [BurstCompile]
        public void Execute(ref MoveData moveData, ref LocalTransform localTrans, ref PhysicsVelocity velocity) {
            // RESET VELOCITY FIRST
            velocity.Linear.AssignKeepY(float3.zero);
            velocity.Angular = float3.zero;

            // DO MOVE
            if (!moveData.isMoveDone) {
                float  moveSpeed  = moveData.moveSpeed;
                float3 moveVector = (moveData.targetLocPos.Full - localTrans.Position).WithoutY();
                float  moveDis    = math.length(moveVector);
                if (moveDis <= moveSpeed * deltaTime) moveData.MarkMoveDone();
                else {
                    velocity.Linear.AssignKeepY(moveSpeed / moveDis * moveVector);

                    // ROTATE RECALCULATING ONLY IF MOVING
                    moveData.RotateTo(moveVector.Quantizate3().xz);
                }
            }

            // DO ROTATE
            quaternion rotateTarget = quaternion.LookRotation(moveData.targetLocDir.Full, math.up());
            float      rotateVecY   = mathHelpers.EulerDiff(localTrans.Rotation, rotateTarget).y;
            float      rotateDis    = math.abs(rotateVecY);
            if (rotateDis <= rotateSpeed * deltaTime)
                localTrans.Rotation = rotateTarget;
            else velocity.Angular.y = rotateSpeed / rotateDis * rotateVecY;
        }
    }

    [WithAll(typeof(Simulate))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [WithDisabled(typeof(MoveableTag))]
    [BurstCompile]
    public partial struct StopDisabledMoveJob : IJobEntity {
        public void Execute(ref PhysicsVelocity velocity) {
            velocity.Angular = float3.zero;
            velocity.Linear.AssignKeepY(float3.zero);
        }
    }
}