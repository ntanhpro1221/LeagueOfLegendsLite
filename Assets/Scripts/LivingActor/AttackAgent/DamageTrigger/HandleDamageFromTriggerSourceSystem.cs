using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateAfter(typeof(UpdateCollidedOpponentSystem))]
public partial struct HandleDamageFromTriggerSourceSystem : ISystem {
    private            BufferLookup<IncomingDamageBuffer> incomingDmgLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();

        incomingDmgLookup = SystemAPI.GetBufferLookup<IncomingDamageBuffer>(
            isReadOnly: false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        incomingDmgLookup.Update(ref state);

        // APPLY TARGETED DAMAGE
        state.Dependency = new ApplyTargetedDamageJob {
            incomingDmgLookup = incomingDmgLookup
        }.Schedule(state.Dependency);

        // APPLY BLOCKABLE DAMAGE
        state.Dependency = new ApplyBlockableDamageJob {
            incomingDmgLookup = incomingDmgLookup
        }.Schedule(state.Dependency);

        // APPLY NON-BLOCKABLE DAMAGE
        state.Dependency = new ApplyNonBlockableDamageJob {
            incomingDmgLookup = incomingDmgLookup
        }.Schedule(state.Dependency);
    }

    #region APPLY TARGETED DAMAGE

    [WithAll(
        typeof(Simulate)
      , typeof(DamageTriggerSource.TargetedTag))]
    [WithDisabled(
        typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct ApplyTargetedDamageJob : IJobEntity {
        public            BufferLookup<IncomingDamageBuffer> incomingDmgLookup;

        [BurstCompile]
        public void Execute(
            in DamageTriggerSource            damageData
          , in AimedTargetData                targetData
            , in MoveData moveData
          , EnabledRefRW<NetworkDestroyedTag> destroy) {
            
            if (!moveData.isMoveDone) return;

            incomingDmgLookup[targetData.target].Add(new IncomingDamageBuffer(damageData.damage));

            destroy.ValueRW = true;
        }
    }

    #endregion

    #region APPLY BLOCKABLE DAMAGE

    [WithAll(
        typeof(Simulate)
      , typeof(DamageTriggerSource.ShotBlockableTag))]
    [WithDisabled(
        typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct ApplyBlockableDamageJob : IJobEntity {
        public BufferLookup<IncomingDamageBuffer> incomingDmgLookup;

        [BurstCompile]
        public void Execute(
            in DamageTriggerSource                   damageData
          , in DynamicBuffer<CollidedOpponentBuffer> collidedOpponent
          , in Entity                                entity
          , EnabledRefRW<NetworkDestroyedTag>        destroy) {

            // there is no opponent to damage
            if (collidedOpponent.Length == 0) return;

            // handle only one entity here
            incomingDmgLookup[collidedOpponent[0].entity].Add(new IncomingDamageBuffer(damageData.damage));

            destroy.ValueRW = true;
        }
    }

    #endregion

    #region APPLY NON-BLOCKABLE DAMAGE

    [WithAll(
        typeof(Simulate)
      , typeof(DamageTriggerSource.ShotNonBlockableTag))]
    [WithDisabled(
        typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct ApplyNonBlockableDamageJob : IJobEntity {
        public BufferLookup<IncomingDamageBuffer> incomingDmgLookup;

        [BurstCompile]
        public void Execute(
            in  DamageTriggerSource                   damageData
          , in  DynamicBuffer<CollidedOpponentBuffer> collidedOpponent
          , ref DamagedOpponentCount                  damagedCount) {
            // already damaged all collided opponent
            if (collidedOpponent.Length == damagedCount.count) return;

            for (int i = damagedCount.count + 1; i <= collidedOpponent.Length; ++i)
                incomingDmgLookup[collidedOpponent[i].entity]
                    .Add(new IncomingDamageBuffer(damageData.damage));

            damagedCount.count = collidedOpponent.Length;
        }
    }

    #endregion
}