using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(CollidedOpponentAuthoring))]
public class DamageTriggerSource_ShotBlockableAuthoring : 
    IDamageTriggerSourceAuthoring<DamageTriggerSource.Type.ShotBlockable> {
    private class Baker : Baker<DamageTriggerSource_ShotBlockableAuthoring> {
        public override void Bake(DamageTriggerSource_ShotBlockableAuthoring authoring) {
            authoring.BakeTriggerSourceBase(this);
        }
    }
}