using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(OwnerSendType = SendToOwnerType.SendToNonOwner)]
public struct MoveInputData : IInputComponentData {
    [GhostField(Quantization = 0)] public float3 targetPos;
}

public class MoveAuthoring : MonoBehaviour {
    private class Baker : Baker<MoveAuthoring> {
        public override void Bake(MoveAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MoveInputData>(entity);
        }
    }
}