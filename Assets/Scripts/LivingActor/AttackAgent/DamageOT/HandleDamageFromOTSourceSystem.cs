using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
public partial struct HandleDamageFromOTSourceSystem : ISystem {
    [BurstCompile]
    private void CalculateTotalDamage(ref SystemState state) {
        var deltaTime = SystemAPI.Time.fixedDeltaTime;
        foreach (var damageOTSource
            in SystemAPI
                .Query<RefRW<DamageOTSource>>()
                .WithAll<Simulate>()) {
            float totalDeltaTime = deltaTime + damageOTSource.ValueRO.timeResidual;
            int applyTimes = (int)math.floor(
                totalDeltaTime / damageOTSource.ValueRO.interval);

            damageOTSource.ValueRW.timeResidual   = new(totalDeltaTime - applyTimes * damageOTSource.ValueRO.interval);
            damageOTSource.ValueRW.tmpTotalDamage = damageOTSource.ValueRO.damageOT * applyTimes;
        }
    }

    [BurstCompile]
    private void ApplyAreaDamage(ref SystemState state) {
        if (!SystemAPI.TryGetSingleton<PhysicsWorldSingleton>(out var physicsWorld)) return;

        var collisionWorld = physicsWorld.CollisionWorld;
        foreach (var (
                damageOTSource
              , locTrans
              , collider
              , teamType)
            in SystemAPI
                .Query<
                    RefRW<DamageOTSource>
                  , RefRO<LocalTransform>
                  , RefRO<PhysicsCollider>
                  , RefRO<TeamTypeData>>()
                .WithAll<
                    DamageAreaTag
                  , Simulate>()) {
            float3 pos = locTrans.ValueRO.Position;
            castResult.Clear();
            if (!collisionWorld.CastCollider(
                new ColliderCastInput(collider.ValueRO.Value, pos, pos)
              , ref castResult)) continue;

            foreach (var hit in castResult) {
                if (!SystemAPI.HasComponent<TeamTypeData>(hit.Entity)
                 || SystemAPI.GetComponent<TeamTypeData>(hit.Entity).team == teamType.ValueRO.team)
                    continue;

                if (!SystemAPI.HasBuffer<IncomingDamageBuffer>(hit.Entity)) continue;
                
                var damageBuffer = SystemAPI.GetBuffer<IncomingDamageBuffer>(hit.Entity);
                damageBuffer.Add(new IncomingDamageBuffer { damage = damageOTSource.ValueRO.tmpTotalDamage });
            }
        }
    }

    [BurstCompile]
    private void ApplyTargetDamage(ref SystemState state) {
        foreach (var (
                damageOTSource
              , targetTag)
            in SystemAPI.Query<
                    RefRO<DamageOTSource>
                  , RefRO<AimedTargetData>>()
                .WithAll<Simulate>())
            SystemAPI.GetBuffer<IncomingDamageBuffer>(targetTag.ValueRO.target).Add(new() {
                damage = damageOTSource.ValueRO.tmpTotalDamage
            });
    }

    private NativeList<ColliderCastHit> castResult;
     
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        castResult = new NativeList<ColliderCastHit>(Allocator.Persistent);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;
        
        CalculateTotalDamage(ref state); // MUST BE RUN FIRST ⚠️⚠️⚠️⚠️

        ApplyAreaDamage(ref state);
          
        ApplyTargetDamage(ref state);
    }
     
    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        castResult.Dispose();
    }
}