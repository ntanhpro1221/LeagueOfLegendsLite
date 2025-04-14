using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HighlightData : IComponentData {
    public bool isHighlighted;

    public HighlightData(bool isHighlighted) => this.isHighlighted = isHighlighted;
}

[GhostEnabledBit]
public struct HighlightVisible : IComponentData, IEnableableComponent { }

public class HighlightableAuthoring : MonoBehaviour {
    private class Baker : Baker<HighlightableAuthoring> {
        public override void Bake(HighlightableAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<HighlightData>(entity);
            AddComponent<HighlightVisible>(entity);
        }
    }
}