using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct RotationData : IComponentData {
    [GhostField] public floatXZ_Q3 rotation;
    [GhostField] public float4_Q3  quaternion;
    
    public void StopRotate()             => rotation = floatXZ_Q3.zero;

    public void RotateTo(floatXZ_Q3 dir) {
        if (dir.Equals(rotation)) return;
        rotation   = dir;
        quaternion = (float4_Q3)Unity.Mathematics.quaternion.LookRotation(dir.Full, math.up());
    }

    [GhostEnabledBit]
    public struct ApplyToEntity : IComponentData, IEnableableComponent { }
}

public class RotationAuthoring : MonoBehaviour {
    public bool applyToEntity;

    private class Baker : ExtendBaker<RotationAuthoring> {
        public override void Bake(RotationAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<RotationData>(entity);
            AddComponent<RotationData.ApplyToEntity>(entity);
            SetComponentEnabled<RotationData.ApplyToEntity>(entity, authoring.applyToEntity);
        }
    }
}