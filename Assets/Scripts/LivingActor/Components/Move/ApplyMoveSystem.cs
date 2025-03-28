using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
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
                .WithAll<Simulate>())
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
                .WithNone<NetworkDestroyedTag>()) {
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
                .WithNone<
                    NetworkDestroyedTag
                  , Parent>()) {
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
              , localTrans
              , velocity)
            in SystemAPI.Query<
                    RefRO<MoveInputData>
                  , RefRW<LocalTransform>
                  , RefRW<PhysicsVelocity>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>()) {
            if (moveData.ValueRO.targetLocalPos.Equals(float3_Q3.zero)) continue;

            // RESET VELOCITY FIRST
            velocity.ValueRW = PhysicsVelocity.Zero;

            // CACHE SOME VALUE
            int moveSpeed = moveData.ValueRO.moveSpeed;
            var   targetPos = moveData.ValueRO.targetLocalPos;

            // MOVE CALCULATE
            float3 moveVector = targetPos - localTrans.ValueRO.Position;
            moveVector.y = 0;
            float moveDistance = math.length(moveVector);

            if (moveDistance <= moveSpeed * deltaTime)
                localTrans.ValueRW.Position = targetPos;
            else {
                var newVelocity = math.normalize(moveVector) * moveSpeed;
                newVelocity.y           = velocity.ValueRO.Linear.y;
                velocity.ValueRW.Linear = newVelocity;

                quaternion targetRotate = quaternion.LookRotation(moveVector, math.up());
                if (moveData.ValueRO.notUseSmoothRotate)
                    localTrans.ValueRW.Rotation = targetRotate;
                else {
                    float3 rotateVector = math.Euler(math.mul(
                        targetRotate
                      , math.inverse(localTrans.ValueRO.Rotation)));
                    float rotateDistance = math.length(rotateVector);
                    if (rotateDistance <= rotateSpeed * deltaTime)
                        localTrans.ValueRW.Rotation = targetRotate;
                    else
                        velocity.ValueRW.Angular = math.normalize(rotateVector) * rotateSpeed;
                }
            }
        }
    }
}