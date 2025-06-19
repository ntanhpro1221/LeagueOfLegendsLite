using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridModelInitRequest : IComponentData {
    public UnityObjectRef<GameObject> prefabRef;
}

/// <summary>
/// This is not <see cref="GhostPrefabType.Client"/> because we query it in <see cref="HandleEffectIOSystem"/>
/// </summary>
public struct KnockUpFakeTriggerData : IComponentData {
    public NetworkTick endAtTick;
}

/// <summary>
/// <inheritdoc cref="KnockUpFakeTriggerData"/>
/// </summary>
public struct KnockUpFakeTrigger : IComponentData, IEnableableComponent { }

[RequireComponent(
    typeof(SharedAnimAuthoring)
  , typeof(HighlightableAuthoring)
  , typeof(TeamTypeAuthoring))]
[RequireComponent(
    typeof(RotationAuthoring))]
public class HybridModelInitAuthoring : MonoBehaviour {
    public GameObject modelPrefab;

    private class Baker : ExtendBaker<HybridModelInitAuthoring> {
        public override void Bake(HybridModelInitAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new HybridModelInitRequest { prefabRef = authoring.modelPrefab });
            AddComponent<KnockUpFakeTriggerData>(entity);
            AddComponentDisabled<KnockUpFakeTrigger>(entity);
        }
    }
}