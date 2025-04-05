using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
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
        ApplyMove(ref state);
        StopDisabledMove(ref state);
    }

    [BurstCompile]
    private void ApplyMove(ref SystemState state) {
        var   gameRules   = SystemAPI.GetSingleton<CommonGameRulesData>();
        float rotateSpeed = gameRules.rotateSpeed;

        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (
                moveData
              , localTrans
              , velocity)
            in SystemAPI.Query<
                    RefRO<MoveData>
                  , RefRW<LocalTransform>
                  , RefRW<PhysicsVelocity>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>()) {
            // RESET VELOCITY FIRST
            float prevVelocityY = velocity.ValueRW.Linear.y;
            velocity.ValueRW = PhysicsVelocity.Zero;

            // MOVE CALCULATE
            float  moveSpeed  = moveData.ValueRO.moveSpeed;
            float3 moveTarget = moveData.ValueRO.targetLocalPos;
            float3 moveVector = (moveTarget - localTrans.ValueRO.Position).WithoutY();
            float  moveDis    = math.length(moveVector);
            if (moveDis <= moveSpeed * deltaTime)
                localTrans.ValueRW.Position = moveTarget;
            else {
                velocity.ValueRW.Linear = moveSpeed / moveDis * moveVector;

                // ROTATE CALCULATING ONLY IF MOVING
                quaternion rotateTarget = quaternion.LookRotation(moveVector, math.up());
                float3     rotateVector = mathHelpers.EulerDiff(localTrans.ValueRO.Rotation, rotateTarget).JustY();
                float      rotateDis    = math.length(rotateVector);
                if (rotateDis <= rotateSpeed * deltaTime)
                    localTrans.ValueRW.Rotation = rotateTarget;
                else
                    velocity.ValueRW.Angular = rotateSpeed / rotateDis * rotateVector;
            }

            // RESTORE Y VELOCITY (controlled by something such as gravity, etc.)
            velocity.ValueRW.Linear.y = prevVelocityY;
        }
    }

    [BurstCompile]
    private void StopDisabledMove(ref SystemState state) {
        foreach (var velocity in SystemAPI
            .Query<RefRW<PhysicsVelocity>>()
            .WithAll<Simulate>()
            .WithNone<NetworkDestroyedTag>()
            .WithDisabled<MoveData>()) {

            float prevVelocityY = velocity.ValueRW.Linear.y;

            velocity.ValueRW = PhysicsVelocity.Zero;

            velocity.ValueRW.Linear.y = prevVelocityY;
        }
    }
}