using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridHealthBarInitRequest : IComponentData, IEnableableComponent {
    public float       deltaY;
    public HealthBarId healthBarType;
}

[GhostEnabledBit]
public struct HybridHealthBarVisible : IComponentData, IEnableableComponent { }

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridHealthBarData : IComponentData, IEnableableComponent {
    public float                         deltaY;
    public UnityObjectRef<RectTransform> transRef;
    public UnityObjectRef<HealthBarUI>   UIRef;
}

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridHealthBarCleanup : ICleanupComponentData {
    public UnityObjectRef<GameObject> healthBarRef;
}

[RequireComponent(typeof(StatsAuthoring))]
public class HybridHealthBarInitAuthoring : MonoBehaviour {
    public float       deltaY;
    public HealthBarId healthBarType;

    private class Baker : ExtendBaker<HybridHealthBarInitAuthoring> {
        public override void Bake(HybridHealthBarInitAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new HybridHealthBarInitRequest {
                deltaY        = authoring.deltaY
              , healthBarType = authoring.healthBarType
            });
            AddComponent<HybridHealthBarVisible>(entity);
            AddComponentDisabled<HybridHealthBarData>(entity);
        }
    }
}