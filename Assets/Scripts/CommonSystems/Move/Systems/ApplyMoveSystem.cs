using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

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
            Calc(ref moveData, ref localTrans, out var newLinear, out var newAngular);
            
            // APPLY VELOCITY
            GameHelpers.AssignLinearVelocity(ref velocity, newLinear, moveData.controlYAxis);
            velocity.Angular = newAngular;
        }

        [BurstCompile]
        public void Calc(ref MoveData moveData, ref LocalTransform localTrans, out float3 newLinear, out float3 newAngular) {
            newLinear = newAngular = float3.zero;

            // DO MOVE
            if (!moveData.isMoveDone) {
                float  moveSpeed        = moveData.moveSpeed;
                float3 moveVector       = moveData.targetLocPos - localTrans.Position;
                float  moveDis_WithoutY = math.length(moveVector.WithoutY());
                if (moveDis_WithoutY <= moveSpeed * deltaTime) moveData.MarkMoveDone();
                else {
                    // Yes, this is correct: here I use moveDis_WithoutY
                    // Because velocity is only exactly for X and Z
                    newLinear = moveSpeed / moveDis_WithoutY * moveVector;

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
            else newAngular.y       = rotateSpeed / rotateDis * rotateVecY;
        }
    }

    [WithAll(typeof(Simulate))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [WithDisabled(typeof(MoveableTag))]
    [BurstCompile]
    public partial struct StopDisabledMoveJob : IJobEntity {
        public void Execute(ref PhysicsVelocity velocity, in MoveData moveData) {
            GameHelpers.AssignLinearVelocity(ref velocity, float3.zero, moveData.controlYAxis);
            velocity.Angular = float3.zero;
        }
    }
}