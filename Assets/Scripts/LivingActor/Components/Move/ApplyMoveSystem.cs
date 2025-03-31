using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(PredictedFixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(CopyCommandBufferToInputSystemGroup))]
public partial struct ApplyMoveSystem : ISystem {
    private ComponentLookup<LocalTransform>      localTransformLookup;
    private ComponentLookup<Parent>              parentLookup;
    private ComponentLookup<PostTransformMatrix> scaleLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        parentLookup         = SystemAPI.GetComponentLookup<Parent>();
        scaleLookup          = SystemAPI.GetComponentLookup<PostTransformMatrix>();

        state.RequireForUpdate<CommonGameRulesData>();
        state.RequireForUpdate<EnumIndexData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        localTransformLookup.Update(ref state);
        parentLookup.Update(ref state);
        scaleLookup.Update(ref state);

        TryGetMoveSpeedFromStats(ref state);
        TryGetTargetPos(ref state);
        ApplyMove(ref state);
    }

    [BurstCompile]
    private void TryGetMoveSpeedFromStats(ref SystemState state) {
        var moveSpeedId = SystemAPI.GetSingleton<EnumIndexData>().ChampionStatsType[ChampionStatsType.MoveSpeed];

        foreach (var (
                moveData
              , statsData)
            in SystemAPI.Query<
                    RefRW<MoveInputData>
                  , DynamicBuffer<StatsData>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>()
                .WithDisabled<MoveDisabled>())
            moveData.ValueRW.moveSpeed = statsData[moveSpeedId].FullValue;
    }

    private void GetWorldTransformMatrix(in Entity entity, out float4x4 result)
        => TransformHelpers.ComputeWorldTransformMatrix(
            entity
          , out result
          , ref localTransformLookup
          , ref parentLookup
          , ref scaleLookup);

    [BurstCompile]
    private void TryGetTargetPos(ref SystemState state) {
        foreach (var (
                moveData
              , targetData
              , parent)
            in SystemAPI.Query<
                    RefRW<MoveInputData>
                  , RefRO<DamageTargetData>
                  , RefRO<Parent>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>()
                .WithDisabled<MoveDisabled>()) {
            GetWorldTransformMatrix(targetData.ValueRO.target, out var targetWorldMatrix);
            var targetWorldPos = targetWorldMatrix.TransformPoint(float3.zero);
            GetWorldTransformMatrix(parent.ValueRO.Value, out var thisWorldMatrix);

            moveData.ValueRW.targetLocalPos = (float3_Q3)thisWorldMatrix.InverseTransformPoint(targetWorldPos);
        }

        foreach (var (
                moveData
              , targetData)
            in SystemAPI.Query<
                    RefRW<MoveInputData>
                  , RefRO<DamageTargetData>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag, Parent>()
                .WithDisabled<MoveDisabled>()) {
            GetWorldTransformMatrix(targetData.ValueRO.target, out var targetWorldMatrix);
            var targetWorldPos = targetWorldMatrix.TransformPoint(float3.zero);

            moveData.ValueRW.targetLocalPos = (float3_Q3)targetWorldPos;
        }
    }

    [BurstCompile]
    private void ApplyMove(ref SystemState state) {
        var   gameRules   = SystemAPI.GetSingleton<CommonGameRulesData>();
        float rotateSpeed = gameRules.rotateSpeed;

        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (
                moveData
              , moveDisable
              , localTrans
              , velocity)
            in SystemAPI.Query<
                    RefRO<MoveInputData>
                  , EnabledRefRO<MoveDisabled>
                  , RefRW<LocalTransform>
                  , RefRW<PhysicsVelocity>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>()
                .WithPresent<MoveDisabled>()) {
            // RESET VELOCITY FIRST
            float prevVelocityY = velocity.ValueRW.Linear.y;
            velocity.ValueRW = PhysicsVelocity.Zero;

            if (!moveDisable.ValueRO
             && !moveData.ValueRO.targetLocalPos.Equals(float3_Q3.zero)) {
                // MOVE CALCULATE
                float moveSpeed  = moveData.ValueRO.moveSpeed;
                float3   moveTarget = moveData.ValueRO.targetLocalPos;
                float3   moveVector = (moveTarget - localTrans.ValueRO.Position).WithoutY();
                float    moveDis    = math.length(moveVector);
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
            }

            // RESTORE Y VELOCITY (controlled by something such as gravity, etc.)
            velocity.ValueRW.Linear.y = prevVelocityY;
        }
    }
}