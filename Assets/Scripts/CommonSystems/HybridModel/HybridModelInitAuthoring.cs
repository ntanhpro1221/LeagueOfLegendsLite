using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridModelInitRequest : IComponentData {
    public UnityObjectRef<GameObject> prefabRef;
}

[RequireComponent(typeof(SharedAnimAuthoring))]
[RequireComponent(typeof(HighlightableAuthoring))]
[RequireComponent(typeof(SkillPreviewAuthoring))]
[RequireComponent(typeof(TeamTypeAuthoring))]
public class HybridModelInitAuthoring : MonoBehaviour {
    public GameObject modelPrefab;

    private class Baker : Baker<HybridModelInitAuthoring> {
        public override void Bake(HybridModelInitAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HybridModelInitRequest {
                prefabRef = authoring.modelPrefab
            });
        }
    }
}