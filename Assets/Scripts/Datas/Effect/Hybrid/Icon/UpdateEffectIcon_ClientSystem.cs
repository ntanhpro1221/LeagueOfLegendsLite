using Unity.Entities;

[UpdateInGroup(typeof(HandleEffectClientUISystemGroup))]
public partial struct UpdateEffectIcon_ClientSystem : ISystem {
    public void OnUpdate(ref SystemState state) {
        foreach (var (
            effectBuffer
          , effectHash
          , hybrid
            ) in SystemAPI
            .Query<
                DynamicBuffer<EffectBuffer>
              , RefRO<EffectBufferHashData>
              , RefRO<HybridHealthBarData>
            >())
            if (effectHash.ValueRO.NeedFix)
                hybrid.ValueRO.dynamic.effectIconRef.Value.FixAllUI(effectBuffer);
    }
}