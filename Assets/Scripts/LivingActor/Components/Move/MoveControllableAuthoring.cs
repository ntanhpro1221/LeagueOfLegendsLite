using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(OwnerSendType = SendToOwnerType.SendToNonOwner)]
public struct MoveInputData : IInputComponentData {
    [GhostField] public float3_Q3 targetLocalPos;
    [GhostField] public bool      initialized;
}

[GhostEnabledBit]
public struct MoveControlDisabled : IComponentData, IEnableableComponent { }

[RequireComponent(typeof(MoveableAuthoring))]
public class MoveControllableAuthoring : MonoBehaviour {
    public new bool enabled;

    private class Baker : Baker<MoveControllableAuthoring> {
        public override void Bake(MoveControllableAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MoveInputData>(entity);
            AddComponent<MoveControlDisabled>(entity);
            SetComponentEnabled<MoveControlDisabled>(entity, !authoring.enabled);
        }
    }
}