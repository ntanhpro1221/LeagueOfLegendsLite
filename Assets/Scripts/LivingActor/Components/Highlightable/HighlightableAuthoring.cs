using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HighlightData : IComponentData {
    public bool isHighlighted;
}

public class HighlightableAuthoring : MonoBehaviour {
    private class Baker : Baker<HighlightableAuthoring> {
        public override void Bake(HighlightableAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<HighlightData>(entity);
        }
    }
}