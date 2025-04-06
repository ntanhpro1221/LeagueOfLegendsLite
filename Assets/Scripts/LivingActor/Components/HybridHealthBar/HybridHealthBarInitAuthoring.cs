using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridHealthBarInitRequest : IComponentData {
    public float                      deltaY;
    public UnityObjectRef<GameObject> healthBarPrefab;
}

[RequireComponent(typeof(StatsAuthoring))]
public class HybridHealthBarInitAuthoring : MonoBehaviour {
    public float      deltaY;
    public GameObject healthBarPrefab;

    private class Baker : Baker<HybridHealthBarInitAuthoring> {
        public override void Bake(HybridHealthBarInitAuthoring authoring) {

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HybridHealthBarInitRequest {
                deltaY          = authoring.deltaY
              , healthBarPrefab = authoring.healthBarPrefab
            });
        }
    }
}