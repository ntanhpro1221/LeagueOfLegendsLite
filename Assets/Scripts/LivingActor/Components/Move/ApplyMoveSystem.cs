using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct ApplyMoveSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<CommonGameRulesData>();
        state.RequireForUpdate<EnumIndexData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ref var statsEnumId    = ref SystemAPI.GetSingleton<EnumIndexData>().ChampionStatsType;
        
        var     gameRules      = SystemAPI.GetSingleton<CommonGameRulesData>();
        float   rotateSpeed    = gameRules.rotateSpeed;
        float   scaleMoveSpeed = gameRules.scaleMoveSpeed;
        
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        foreach (var (
                moveData
              , localTrans
              , statsData
              , velocity)
            in SystemAPI.Query<
                    RefRO<MoveInputData>
                  , RefRW<LocalTransform>
                  , DynamicBuffer<StatsData>
                  , RefRW<PhysicsVelocity>>()
                .WithAll<Simulate>()) {
            if (moveData.ValueRO.targetPos.Equals(float3.zero)) continue;
            
            // RESET VELOCITY FIRST
            velocity.ValueRW = PhysicsVelocity.Zero;

            // CACHE SOME VALUE
            float  moveSpeed = scaleMoveSpeed * statsData[statsEnumId[ChampionStatsType.MoveSpeed]].Value;
            float3 targetPos = moveData.ValueRO.targetPos;

            // MOVE CALCULATE
            float3 moveVector   = targetPos - localTrans.ValueRO.Position;
            moveVector.y = 0;
            float  moveDistance = math.length(moveVector);
            if (moveDistance <= moveSpeed * deltaTime)
                localTrans.ValueRW.Position = targetPos;
            else {
                // APPLY VELOCITY
                velocity.ValueRW.Linear = math.normalize(moveVector) * moveSpeed;
                
                // ROTATE CALCULATE
                quaternion targetRotate = quaternion.LookRotation(moveVector, math.up());
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