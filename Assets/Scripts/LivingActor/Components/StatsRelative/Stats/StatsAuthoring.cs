using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct StatsBuffer : IBufferElementData, IEnableableComponent {
    [GhostField] public float_Q3 value;

    public static implicit operator StatsBuffer(float_Q3 source) =>
        new() { value = source };
}

[GhostEnabledBit]
public struct StatsBuffer_Raw : IBufferElementData, IEnableableComponent {
    [GhostField] public float_Q3 value;

    public static implicit operator StatsBuffer_Raw(float_Q3 source) =>
        new() { value = source };
}

[GhostEnabledBit]
public struct StatsBuffer_RawPerLevel : IBufferElementData, IEnableableComponent {
    [GhostField] public float_Q3 value;

    public static implicit operator StatsBuffer_RawPerLevel(float_Q3 source) =>
        new() { value = source };
}

public class StatsAuthoring : MonoBehaviour {
    public bool haveLevel;

    private class Baker : ExtendBaker<StatsAuthoring> {
        public override void Bake(StatsAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddCleanBufferDisabled<StatsBuffer>(entity, EnumCount.Stats);
            AddCleanBufferDisabled<StatsBuffer_Raw>(entity, EnumCount.Stats);

            if (authoring.haveLevel)
                AddCleanBufferDisabled<StatsBuffer_RawPerLevel>(entity, EnumCount.Stats);
        }
    }
}