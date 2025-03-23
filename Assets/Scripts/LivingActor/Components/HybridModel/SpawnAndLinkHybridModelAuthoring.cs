using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct SpawnAndLinkHybridModelRequest : IComponentData {
    public UnityObjectRef<GameObject> prefabRef;
}

public class SpawnAndLinkHybridModelAuthoring : MonoBehaviour {
    public GameObject modelPrefab;

    class Baker : Baker<SpawnAndLinkHybridModelAuthoring> {
        public override void Bake(SpawnAndLinkHybridModelAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnAndLinkHybridModelRequest {
                prefabRef = authoring.modelPrefab
            });
        }
    }
}