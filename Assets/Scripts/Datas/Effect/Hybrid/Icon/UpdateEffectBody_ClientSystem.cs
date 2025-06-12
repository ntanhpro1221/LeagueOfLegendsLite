using Unity.Entities;

[UpdateInGroup(typeof(HandleEffectClientUISystemGroup))]
public partial struct UpdateEffectBody_ClientSystem : ISystem {
    public void OnUpdate(ref SystemState state) {
        foreach (var (
            effectBuffer
          , effectHash
          , hybrid
            ) in SystemAPI
            .Query<
                DynamicBuffer<EffectBuffer>
              , RefRO<EffectBufferHashData>
              , RefRO<HybridModelData>
            >())
            if (effectHash.ValueRO.NeedFix)
                hybrid.ValueRO.effectBody.Value.FixAllUI(effectBuffer);
    }
}