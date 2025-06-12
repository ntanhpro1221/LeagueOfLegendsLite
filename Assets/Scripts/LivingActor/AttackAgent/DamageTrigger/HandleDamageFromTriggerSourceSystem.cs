using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct HandleDamageFromTriggerSourceSystem : ISystem {
    private BufferLookup<IncomingDamageBuffer> incomingDmgLookup;
    private BufferLookup<IncomingEffectBuffer> incomingEffectLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        incomingDmgLookup = SystemAPI.GetBufferLookup<IncomingDamageBuffer>(
            isReadOnly: false);
        incomingEffectLookup = SystemAPI.GetBufferLookup<IncomingEffectBuffer>(
            isReadOnly: false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        incomingDmgLookup.Update(ref state);
        incomingEffectLookup.Update(ref state);

        // APPLY TARGETED DAMAGE
        state.Dependency = new ApplyTargetedDamageJob {
            incomingDmgLookup    = incomingDmgLookup
          , incomingEffectLookup = incomingEffectLookup
        }.Schedule(state.Dependency);

        // APPLY BLOCKABLE DAMAGE
        state.Dependency = new ApplyBlockableDamageJob {
            incomingDmgLookup    = incomingDmgLookup
          , incomingEffectLookup = incomingEffectLookup
        }.Schedule(state.Dependency);

        // APPLY NON-BLOCKABLE DAMAGE
        state.Dependency = new ApplyNonBlockableDamageJob {
            incomingDmgLookup    = incomingDmgLookup
          , incomingEffectLookup = incomingEffectLookup
        }.Schedule(state.Dependency);
    }

#region APPLY TARGETED DAMAGE

    [WithAll(
        typeof(Simulate)
      , typeof(DamageTriggerSource.Type.Targeted))]
    [WithDisabled(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct ApplyTargetedDamageJob : IJobEntity {
        public BufferLookup<IncomingDamageBuffer> incomingDmgLookup;
        public BufferLookup<IncomingEffectBuffer> incomingEffectLookup;

        [BurstCompile]
        public void Execute(
            DamageTriggerSourceAspect         damageTrigger
          , in AimedTargetData                targetData
          , in MoveData                       moveData
          , EnabledRefRW<NetworkDestroyedTag> destroyed) {

            if (!moveData.isMoveDone) return;

            damageTrigger.PerformDamageAndEffect(incomingDmgLookup, incomingEffectLookup, targetData.target);

            destroyed.ValueRW = true;
        }
    }

#endregion

#region APPLY BLOCKABLE DAMAGE

    [WithAll(
        typeof(Simulate)
      , typeof(DamageTriggerSource.Type.ShotBlockable))]
    [WithDisabled(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct ApplyBlockableDamageJob : IJobEntity {
        public BufferLookup<IncomingDamageBuffer> incomingDmgLookup;
        public BufferLookup<IncomingEffectBuffer> incomingEffectLookup;

        [BurstCompile]
        public void Execute(
            DamageTriggerSourceAspect                damageTrigger
          , in DynamicBuffer<CollidedOpponentBuffer> collidedOpponent
          , in Entity                                entity
          , EnabledRefRW<NetworkDestroyedTag>        destroyed) {

            // there is no opponent to damage
            if (collidedOpponent.Length == 0) return;

            // handle only one entity here
            damageTrigger.PerformDamageAndEffect(incomingDmgLookup, incomingEffectLookup, collidedOpponent[0].entity);

            destroyed.ValueRW = true;
        }
    }

#endregion

#region APPLY NON-BLOCKABLE DAMAGE

    [WithAll(
        typeof(Simulate)
      , typeof(DamageTriggerSource.Type.ShotNonBlockable))]
    [WithDisabled(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct ApplyNonBlockableDamageJob : IJobEntity {
        public BufferLookup<IncomingDamageBuffer> incomingDmgLookup;
        public BufferLookup<IncomingEffectBuffer> incomingEffectLookup;

        [BurstCompile]
        public void Execute(
            DamageTriggerSourceAspect                 damageTrigger
          , in  DynamicBuffer<CollidedOpponentBuffer> collidedOpponent
          , ref DamagedOpponentCount                  damagedCount) {

            // already damaged all collided opponent
            if (collidedOpponent.Length == damagedCount.count) return;

            for (int i = damagedCount.count + 1; i <= collidedOpponent.Length; ++i)
                damageTrigger.PerformDamageAndEffect(incomingDmgLookup, incomingEffectLookup, collidedOpponent[i].entity);

            damagedCount.count = collidedOpponent.Length;
        }
    }

#endregion
}