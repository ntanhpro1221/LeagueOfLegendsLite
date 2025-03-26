using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct IncomingDamageBuffer : IBufferElementData {
    [GhostField(Quantization = 0)] public float damage;
}

[RequireComponent(typeof(HealthAuthoring))]
public class DamageableAuthoring : MonoBehaviour {
    private class Baker : Baker<DamageableAuthoring> {
        public override void Bake(DamageableAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddBuffer<IncomingDamageBuffer>(entity);
        }
    }
}
