using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct SpawnHybridModelRequest : IComponentData {
    public UnityObjectRef<GameObject> prefabRef;
}

public class SpawnHybridModelRequestAuthoring : MonoBehaviour {
    public GameObject modelPrefab;

    class Baker : Baker<SpawnHybridModelRequestAuthoring> {
        public override void Bake(SpawnHybridModelRequestAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnHybridModelRequest {
                prefabRef = authoring.modelPrefab
            });
        }
    }
}