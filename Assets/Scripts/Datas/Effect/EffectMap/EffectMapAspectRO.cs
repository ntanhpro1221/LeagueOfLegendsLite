using Unity.Collections;
using Unity.Entities;

public readonly partial struct EffectMapAspectRO : IAspect {
    private readonly RefRO<EffectMap> _Map;

    [ReadOnly] private readonly DynamicBuffer<EffectBuffer> _Buffer;

    public ref readonly EffectMap Map => ref _Map.ValueRO;

    public bool ContainsKey(EffectId id) => _Map.ValueRO.ContainsKey(id);

    public bool TryGetFirstEffect(EffectId id, out EffectBuffer result) {
        if (_Map.ValueRO.TryGetValue(id, out var bufferIndex)) {
            result = _Buffer[bufferIndex];
            return true;
        }

        result = default;
        return false;
    }
}