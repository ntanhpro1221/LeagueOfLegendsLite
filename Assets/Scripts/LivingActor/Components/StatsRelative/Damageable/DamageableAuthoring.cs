using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct IncomingDamageBuffer : IBufferElementData {
    [GhostField] public float_Q3 damage;
    
    public IncomingDamageBuffer(float_Q3 damage) {
        this.damage = damage;
    }
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
