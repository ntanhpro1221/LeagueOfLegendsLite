using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct MoveData : IComponentData, IEnableableComponent {
    [GhostField] public float3_Q3 targetLocalPos;
    [GhostField] public float_Q3  moveSpeed;
}

[RequireComponent(typeof(Rigidbody))]
public class MoveableAuthoring : MonoBehaviour {
    public new bool enabled;

    private class Baker : Baker<MoveableAuthoring> {
        public override void Bake(MoveableAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MoveData>(entity);
            SetComponentEnabled<MoveData>(entity, authoring.enabled);
        }
    }
}