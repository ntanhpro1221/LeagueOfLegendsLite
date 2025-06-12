using Unity.Collections;
using Unity.Entities;

public readonly partial struct DamageTriggerSourceAspect : IAspect {
    private readonly RefRO<DamageTriggerSource> _DamageSource;

    // ReSharper disable once UnassignedReadonlyField
    [ReadOnly] public readonly DynamicBuffer<DamageTriggerSource.EffectBuffer> Effects;

    public void PerformDamageAndEffect(
        in BufferLookup<IncomingDamageBuffer> incomingDmgLookup
      , in BufferLookup<IncomingEffectBuffer> incomingEffectLookup
      , in Entity                             target) {
        ref readonly var damageSource = ref _DamageSource.ValueRO;

        // DAMAGE
        incomingDmgLookup[target].Add(new IncomingDamageBuffer(damageSource.damage, damageSource.source));

        // EFFECT
        if (!Effects.IsEmpty) {
            var incomingEffects = incomingEffectLookup[target];

            var effectPattern = new IncomingEffectBuffer {
                id           = new EffectFullId { source = damageSource.source }
              , senderScaler = damageSource.sourceScaler
              , senderPos    = damageSource.sourcePos
            };

            foreach (var effect in Effects) {
                var newEffect = effectPattern;
                newEffect.id.id          = effect.id;
                newEffect.customLifeTick = effect.customLifeTick;
                incomingEffects.Add(newEffect);
            }
        }
    }
}