using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct AutoFollowTarget : IComponentData, IEnableableComponent { }

[RequireComponent(typeof(MoveableAuthoring))]
[RequireComponent(typeof(AimedTargetAuthoring))]
public class AutoFollowTargetAuthoring : MonoBehaviour {
    public new bool enabled;
    
    private class Baker : ExtendBaker<AutoFollowTargetAuthoring> {
        public override void Bake(AutoFollowTargetAuthoring authoring) {
            GetDynamicEntity(out var entity);
            AddComponent<AutoFollowTarget>(entity);
            SetComponentEnabled<AutoFollowTarget>(entity, authoring.enabled);
        }
    }
}