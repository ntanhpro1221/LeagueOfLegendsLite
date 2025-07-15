using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(NormalAttackSystemGroup))]
public partial struct CommonMeleeAttackSystem : ISystem {
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

        state.Dependency = new Job {
            incomingDmgLookup    = incomingDmgLookup
          , incomingEffectLookup = incomingEffectLookup
        }.Schedule(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public BufferLookup<IncomingDamageBuffer> incomingDmgLookup;
        public BufferLookup<IncomingEffectBuffer> incomingEffectLookup;

        [BurstCompile]
        private void Execute(
            ScalerPersonalConstructAspect                      personalConstructor
          , in DynamicBuffer<DamageTriggerSource.EffectBuffer> onHitEffects
          , in AimedTargetData                                 target
          , in LocalTransform                                  locTrans
          , EnabledRefRW<MeleeAttackTrigger>                   attackTrigger
          , in Entity                                          entity) {

            attackTrigger.ValueRW = false;

            // deal damage
            incomingDmgLookup[target.target].Add(
                new IncomingDamageBuffer(personalConstructor.Stats.PhysicDamage, entity));

            // apply effect
            if (!onHitEffects.IsEmpty) {
                var incomingEffects = incomingEffectLookup[target.target];

                var effectPattern = new IncomingEffectBuffer {
                    id           = new EffectFullId { source = entity }
                  , senderScaler = personalConstructor.Construct()
                  , senderPos    = locTrans.Position.Quantizate3()
                };

                foreach (var effect in onHitEffects) {
                    var newEffect = effectPattern;
                    newEffect.id.id          = effect.id;
                    newEffect.customLifeTick = effect.customLifeTick;
                    incomingEffects.Add(newEffect);
                }
            }
        }
    }
}