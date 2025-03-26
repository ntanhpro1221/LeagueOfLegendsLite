using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct SpawnAndLinkHybridHealthBarRequest : IComponentData {
    public float                      deltaY;
    public UnityObjectRef<GameObject> healthBarPrefab;
}

[RequireComponent(typeof(StatsAuthoring))]
public class SpawnAndLinkHybridHealthBarAuthoring : MonoBehaviour {
    public float      deltaY;
    public GameObject healthBarPrefab;

    private class Baker : Baker<SpawnAndLinkHybridHealthBarAuthoring> {
        public override void Bake(SpawnAndLinkHybridHealthBarAuthoring authoring) {

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnAndLinkHybridHealthBarRequest {
                deltaY          = authoring.deltaY
              , healthBarPrefab = authoring.healthBarPrefab
            });
        }
    }
}