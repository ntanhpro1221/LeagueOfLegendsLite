using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct StatsBuffer : IBufferElementData, IEnableableComponent {
    [GhostField] public float_Q3 value;
}

public struct RawStatsData : ICleanupComponentData {
    public BlobAssetReference<BubleEnMap<StatsType, float_Q3>> _Ref;

    public ref float_Q3 this[StatsType key] => ref _Ref.Value[key];
}

public struct RawStatsPerLevelData : ICleanupComponentData {
    public BlobAssetReference<BubleEnMap<StatsType, float_Q3>> _Ref;

    public ref float_Q3 this[StatsType key] => ref _Ref.Value[key];
}

public struct NeedBuildRawStats : IComponentData { }

public class StatsAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<StatsAuthoring> {
        public override void Bake(StatsAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddCleanBufferDisabled<StatsBuffer>(entity, Enum.GetValues(typeof(StatsType)).Length);
            AddComponent<NeedBuildRawStats>(entity);
        }
    }
}