using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateAfter(typeof(UpdateCollidedOpponentSystem))]
public partial struct HandleDamageFromTriggerSourceSystem : ISystem {
    [ReadOnly] private ComponentLookup<LocalTransform>    locTransLookup;
    private            BufferLookup<IncomingDamageBuffer> incomingDmgLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();

        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
        incomingDmgLookup = SystemAPI.GetBufferLookup<IncomingDamageBuffer>(
            isReadOnly: false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        locTransLookup.Update(ref state);
        incomingDmgLookup.Update(ref state);

        // APPLY TARGETED DAMAGE
        state.Dependency = new ApplyTargetedDamageJob {
            locTransLookup    = locTransLookup
          , incomingDmgLookup = incomingDmgLookup
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
        [ReadOnly] public ComponentLookup<LocalTransform>    locTransLookup;
        public            BufferLookup<IncomingDamageBuffer> incomingDmgLookup;

        [BurstCompile]
        public void Execute(
            in DamageTriggerSource            damageData
          , in AimedTargetData                targetData
          , in LocalTransform                 locTrans
          , EnabledRefRW<NetworkDestroyedTag> destroy) {
            float dis = math.length((locTrans.Position - this.locTransLookup[targetData.target].Position).WithoutY());
            if (dis > float_Q3.EPSILON) return;

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