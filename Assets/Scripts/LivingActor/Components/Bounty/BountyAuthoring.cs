using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// Index = (int)<see cref="BountyType"/>
/// </summary>
public struct BountyData : IComponentData, IEnableableComponent {
    [GhostField] public Strum.Bounty.Fields<float_Q3> data;
}

public struct BountyTrigger : IComponentData, IEnableableComponent { }

public struct BountyTriggerData : IComponentData {
    [GhostField] public Entity lastHitEntity;
}

public class BountyAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<BountyAuthoring> {
        public override void Bake(BountyAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponentDisabled<BountyData>(entity);
            AddComponentDisabled<BountyTrigger>(entity);
            AddComponent<BountyTriggerData>(entity);
        }
    }
}