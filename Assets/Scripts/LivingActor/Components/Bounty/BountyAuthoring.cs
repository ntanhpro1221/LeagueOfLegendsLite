using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// Index = (int)<see cref="BountyType"/>
/// </summary>
public struct BountyBuffer : IBufferElementData, IEnableableComponent {
    [GhostField] public float_Q3 value;

    public static implicit operator BountyBuffer(float_Q3 source) =>
        new() { value = source };
}

public struct BountyTrigger : IComponentData, IEnableableComponent { }

public struct BountyTriggerData : IComponentData {
    [GhostField] public Entity lastHitEntity;
}

public class BountyAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<BountyAuthoring> {
        public override void Bake(BountyAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddCleanBufferDisabled<BountyBuffer>(entity, EnumCount.Bounty);
            AddComponentDisabled<BountyTrigger>(entity);
            AddComponent<BountyTriggerData>(entity);
        }
    }
}