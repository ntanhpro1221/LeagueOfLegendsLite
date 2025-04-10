using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct MoveData : IComponentData, IEnableableComponent {
    [GhostField] public floatXZ_Q3 targetLocalPos;
    [GhostField] public float_Q3   moveSpeed;
    [GhostField] public bool       isDone;

    public void MoveTo(floatXZ_Q3 pos) => (targetLocalPos, isDone) = (pos, false);
    public void MoveTo(float3_Q3  pos) => (targetLocalPos, isDone) = (pos.xz, false);
    public void TeleTo(floatXZ_Q3 pos) => (targetLocalPos, isDone) = (pos, true);
    public void TeleTo(float3_Q3  pos) => (targetLocalPos, isDone) = (pos.xz, true);
    public void MarkDone()             => isDone = true;
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