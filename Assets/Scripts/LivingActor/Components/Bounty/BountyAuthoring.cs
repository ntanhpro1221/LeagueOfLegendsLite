using System;
using NGDtuanh.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public interface IHaveBountyManaged {
    CovEnumMap<BountyId, float_Q3> Bounty { get; }
}

public struct BountyData : IComponentData {
    [GhostField] public Strum.Bounty.Fields<float_Q3> data;
}

public struct BountyTrigger : IComponentData, IEnableableComponent { }

public struct BountyTriggerData : IComponentData {
    [GhostField] public Entity lastHitEntity;
}

/// <summary>
/// Send gold to someone (not by using my gold).
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.Server)]
public struct OutgoingGoldBuffer : IBufferElementData {
    public float_Q3 gold;
    public Entity   target;
}

[RequireComponent(typeof(IRaceTag))]
public class BountyAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<BountyAuthoring> {
        public override void Bake(BountyAuthoring authoring) {
            if (ActorAuthoringHelpers.IsBaseRace(authoring)) return;

            GetDynamicEntity(out var entity);

            AddComponentDisabled<BountyTrigger>(entity);
            AddComponent<BountyTriggerData>(entity);
            AddBuffer<OutgoingGoldBuffer>(entity);

            var source = ActorAuthoringHelpers.ExtractDataFromTag(authoring);
            if (source is not IHaveBountyManaged bountySource)
                throw new Exception(
                    $"NGDtuanh: {authoring.name}'s data must have bounty");
            BountyData bountyData = default;
            foreach (var index in Strum.Bounty.Indexes)
                bountyData.data[index] = bountySource.Bounty[index];
            AddComponent(entity, bountyData);
        }
    }
}