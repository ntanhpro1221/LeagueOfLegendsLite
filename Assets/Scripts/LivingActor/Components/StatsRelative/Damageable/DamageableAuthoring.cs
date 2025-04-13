using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Serialization;

public struct IncomingDamageBuffer : IBufferElementData {
    [GhostField] public float_Q3 damage;
    
    public IncomingDamageBuffer(float_Q3 damage) {
        this.damage = damage;
    }
}

public struct TakeDamageSpot : IComponentData {
    [GhostField] public float3_Q3 spot;
}

[RequireComponent(typeof(HealthAuthoring))]
public class DamageableAuthoring : MonoBehaviour {
    public Transform TakeDamageSpot;

    private class Baker : Baker<DamageableAuthoring> {
        public override void Bake(DamageableAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddBuffer<IncomingDamageBuffer>(entity);
            AddComponent(entity, new TakeDamageSpot {
                spot = (authoring.TakeDamageSpot.position - authoring.transform.position).Quantizate3()
            });
        }
    }
}