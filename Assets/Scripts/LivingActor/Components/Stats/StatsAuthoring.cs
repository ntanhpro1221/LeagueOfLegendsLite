using NGDtuanh.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct StatsData : IBufferElementData, IEnableableComponent {
    [GhostField] public float_Q3 FullValue;
}

public class StatsAuthoring : MonoBehaviour {
    private class Baker : Baker<StatsAuthoring> {
        public override void Bake(StatsAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            this.AddBufferDisabled<StatsData>(entity, new EnumMap<ChampionStatsType, int>().Count);
        }
    }
}