using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct StatsBuffer : IBufferElementData, IEnableableComponent {
    [GhostField] public float_Q3 value;
}

public struct RawStatsData : IComponentData, IEnableableComponent {
    public BlobAssetReference<BubleEnMap<StatsType, float_Q3>> _Ref;

    public ref float_Q3 this[StatsType key] => ref _Ref.Value[key];
}

public struct RawStatsPerLevelData : IComponentData, IEnableableComponent {
    public BlobAssetReference<BubleEnMap<StatsType, float_Q3>> _Ref;

    public ref float_Q3 this[StatsType key] => ref _Ref.Value[key];
}

public class StatsAuthoring : MonoBehaviour {
    public bool haveLevel = true;

    private class Baker : Baker<StatsAuthoring> {
        public override void Bake(StatsAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            this.AddBufferDisabled<StatsBuffer>(entity, Enum.GetValues(typeof(StatsType)).Length);
            this.AddComponentDisabled<RawStatsData>(entity);
            if (authoring.haveLevel)
                this.AddComponentDisabled<RawStatsPerLevelData>(entity);
        }
    }
}