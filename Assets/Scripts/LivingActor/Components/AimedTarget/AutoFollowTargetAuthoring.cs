using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct AutoFollowTarget : IComponentData, IEnableableComponent {
    [GhostField] public Method followMethod;

    public float3 tmpCurDirToTarget;
    public float3 tmpReachableTargetPos;
    public float  tmpYourRange;
    public float  tmpTargetRadius;

    public enum Method {
        Straight    = 0
      , SmartAttack = 1
    }
}

[RequireComponent(typeof(MoveableAuthoring))]
[RequireComponent(typeof(AimedTargetAuthoring))]
public class AutoFollowTargetAuthoring : MonoBehaviour {
    public new bool                    enabled;
    public     AutoFollowTarget.Method followMethod;

    private class Baker : ExtendBaker<AutoFollowTargetAuthoring> {
        public override void Bake(AutoFollowTargetAuthoring authoring) {
            GetDynamicEntity(out var entity);
            AddComponent(entity, new AutoFollowTarget { followMethod = authoring.followMethod });
            SetComponentEnabled<AutoFollowTarget>(entity, authoring.enabled);
        }
    }
}