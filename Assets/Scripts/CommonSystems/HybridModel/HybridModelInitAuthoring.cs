using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridModelData : IComponentData, IEnableableComponent {
    public UnityObjectRef<Transform>            transformRef;
    public UnityObjectRef<SharedAnimController> animCtrlRef;
    public UnityObjectRef<Outline>              outlineRef;
    public UnityObjectRef<SkillPreviewShower>   skillPreviewRef;
}

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridModelCleanupData : ICleanupComponentData {
    public UnityObjectRef<GameObject> objectRef;
}

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridModelInitRequest : IComponentData, IEnableableComponent {
    public UnityObjectRef<GameObject> prefabRef;
}

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct ManualPoolingHybridModel : IComponentData { }

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct ManualPoolingHybridModel_Cleanup : ICleanupComponentData { }

[RequireComponent(typeof(SharedAnimAuthoring))]
[RequireComponent(typeof(HighlightableAuthoring))]
[RequireComponent(typeof(TeamTypeAuthoring))]
public class HybridModelInitAuthoring : MonoBehaviour {
    public GameObject modelPrefab;
    public bool       manualPooling;

    private class Baker : ExtendBaker<HybridModelInitAuthoring> {
        public override void Bake(HybridModelInitAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponentDisabled<HybridModelData>(entity);
            AddComponent(entity, new HybridModelInitRequest {
                prefabRef = authoring.modelPrefab
            });
            
            if (authoring.manualPooling)
                AddComponent<ManualPoolingHybridModel>(entity);
        }
    }
}