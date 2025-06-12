using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(AutoFollowTargetAuthoring))]
public class DamageTriggerSource_TargetedAuthoring : 
    IDamageTriggerSourceAuthoring<DamageTriggerSource.Type.Targeted> {
    private class Baker : Baker<DamageTriggerSource_TargetedAuthoring> {
        public override void Bake(DamageTriggerSource_TargetedAuthoring authoring) {
            authoring.BakeTriggerSourceBase(this);
        }
    }
}