using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct IncomingDamageBuffer : IBufferElementData {
    public float_Q3 damage;
    public Entity   source;

    public IncomingDamageBuffer(float_Q3 damage, Entity source) {
        this.damage = damage;
        this.source = source;
    }
}

public struct AssistBuffer : IBufferElementData {
    [GhostField] public Entity entity;

    public static implicit operator AssistBuffer(Entity entity) =>
        new() { entity = entity };
}

[GhostEnabledBit]
public struct AssistResetTrigger : IComponentData, IEnableableComponent { }

public struct AssistResetData : IComponentData {
    [GhostField] public NetworkTick resetAtTick;
}

public struct TakeDamageSpot : IComponentData {
    [GhostField] public float3_Q3 spot;
}

[RequireComponent(typeof(HealthAuthoring))]
public class DamageableAuthoring : MonoBehaviour {
    public Transform TakeDamageSpot;

    private class Baker : ExtendBaker<DamageableAuthoring> {
        public override void Bake(DamageableAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddBuffer<IncomingDamageBuffer>(entity);
            AddBuffer<AssistBuffer>(entity);
            AddComponentDisabled<AssistResetTrigger>(entity);
            AddComponent<AssistResetData>(entity);
            AddComponent(entity, new TakeDamageSpot {
                spot = (authoring.TakeDamageSpot.position - authoring.transform.position).Quantizate3()
            });
        }
    }
}