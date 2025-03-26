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
    [GhostField(Quantization = 0)] public float3 targetLocalPos;
    [GhostField(Quantization = 0)] public float  moveSpeed;
    [GhostField]                   public bool   notUseSmoothRotate;
}

public class MoveInputAuthoring : MonoBehaviour {
    public float moveSpeed;
    public bool  notUseSmoothRotate;

    private class Baker : Baker<MoveInputAuthoring> {
        public override void Bake(MoveInputAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveInputData {
                moveSpeed          = authoring.moveSpeed
              , notUseSmoothRotate = authoring.notUseSmoothRotate
            });
        }
    }
}