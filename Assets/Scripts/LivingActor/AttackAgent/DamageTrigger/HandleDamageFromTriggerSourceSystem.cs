using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateAfter(typeof(UpdateCollidedOpponentSystem))]
[UpdateBefore(typeof(DestroyNetworkEntityServerSystem))]
public partial struct HandleDamageFromTriggerSourceSystem : ISystem {
    [BurstCompile]
    private void ApplyTargetedDamage(ref SystemState state, ref EntityCommandBuffer ecb) {
        foreach (var (
                damageData
              , targetData
              , localToWorld
              , entity)
            in SystemAPI
                .Query<
                    RefRO<DamageTriggerSource>
                  , RefRO<AimedTargetData>
                  , RefRO<LocalToWorld>>()
                .WithAll<
                    Simulate
                  , DamageTriggerSource.TargetedTag>()
                .WithNone<NetworkDestroyedTag>()
                .WithEntityAccess()) {

            var distance = math.distance(
                localToWorld.ValueRO.Position
              , SystemAPI.GetComponent<LocalToWorld>(targetData.ValueRO.target).Position);
            if (distance > float_Q3.Epsilon) continue;

            SystemAPI.GetBuffer<IncomingDamageBuffer>(targetData.ValueRO.target).Add(new() {
                damage = damageData.ValueRO.damage
            });

            ecb.AddComponent<NetworkDestroyedTag>(entity);
        }
    }

    [BurstCompile]
    private void ApplyBlockableDamage(ref SystemState state, ref EntityCommandBuffer ecb) {
        foreach (var (
                damageData
              , collidedOpponent
              , entity)
            in SystemAPI
                .Query<
                    RefRO<DamageTriggerSource>
                  , DynamicBuffer<CollidedOpponentBuffer>>()
                .WithAll<
                    Simulate
                  , DamageTriggerSource.ShotBlockableTag>()
                .WithNone<NetworkDestroyedTag>()
                .WithEntityAccess()) {
            if (collidedOpponent.Length == 0) continue; // there is no opponent to damage

            // just handle one entity here
            SystemAPI.GetBuffer<IncomingDamageBuffer>(collidedOpponent[0].entity).Add(new() {
                damage = damageData.ValueRO.damage
            });

            ecb.AddComponent<NetworkDestroyedTag>(entity);
        }
    }

    [BurstCompile]
    private void ApplyNonBlockableDamage(ref SystemState state) {
        foreach (var (
                damageData
              , collidedOpponent
              , damagedCount)
            in SystemAPI
                .Query<
                    RefRO<DamageTriggerSource>
                  , DynamicBuffer<CollidedOpponentBuffer>
                  , RefRW<DamagedOpponentCount>>()
                .WithAll<
                    Simulate
                  , DamageTriggerSource.ShotNonBlockableTag>()
                .WithNone<NetworkDestroyedTag>()) {

            if (collidedOpponent.Length == damagedCount.ValueRO.count) continue; // already damaged all collided opponent
            damagedCount.ValueRW.count = collidedOpponent.Length;

            foreach (var opponent in collidedOpponent)
                SystemAPI.GetBuffer<IncomingDamageBuffer>(opponent.entity).Add(new() {
                    damage = damageData.ValueRO.damage
                });
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        ApplyTargetedDamage(ref state, ref ecb);
        ApplyBlockableDamage(ref state, ref ecb);
        ApplyNonBlockableDamage(ref state);
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}