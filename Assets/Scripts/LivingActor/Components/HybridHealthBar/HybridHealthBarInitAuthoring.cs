using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridHealthBarInitRequest : IComponentData {
    public float                      deltaY;
    public UnityObjectRef<GameObject> healthBarPrefab;
}

[GhostEnabledBit]
public struct HybridHealthBarVisible : IComponentData, IEnableableComponent { }

[RequireComponent(typeof(StatsAuthoring))]
public class HybridHealthBarInitAuthoring : MonoBehaviour {
    public float      deltaY;
    public GameObject healthBarPrefab;

    private class Baker : ExtendBaker<HybridHealthBarInitAuthoring> {
        public override void Bake(HybridHealthBarInitAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new HybridHealthBarInitRequest {
                deltaY          = authoring.deltaY
              , healthBarPrefab = authoring.healthBarPrefab
            });
            AddComponent<HybridHealthBarVisible>(entity);
        }
    }
}