using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// - if your entity have <see cref="StatsData"/> component,
/// <see cref="ApplyMoveSystem"/> will get move speed from this instead of <see cref="MoveInputData.moveSpeed"/>.<br/>
/// - if your entity have <see cref="DamageTargetData"/> component,
/// <see cref="ApplyMoveSystem"/> will get target pos from this instead of <see cref="targetLocalPos"/>.<br/>
/// </summary>
[GhostComponent(OwnerSendType = SendToOwnerType.SendToNonOwner)]
public struct MoveInputData : IInputComponentData {
    [GhostField] public float3_Q3 targetLocalPos;
    [GhostField] public float_Q3  moveSpeed;
}

[GhostEnabledBit]
public struct MoveDisabled : IComponentData, IEnableableComponent { }

public class MoveInputAuthoring : MonoBehaviour {
    public float_Q3 moveSpeed;

    private class Baker : Baker<MoveInputAuthoring> {
        public override void Bake(MoveInputAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveInputData {
                moveSpeed = authoring.moveSpeed
            });
            this.AddComponentDisabled<MoveDisabled>(entity);
        }
    }
}