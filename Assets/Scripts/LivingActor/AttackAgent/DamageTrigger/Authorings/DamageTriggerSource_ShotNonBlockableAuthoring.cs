using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct DamagedOpponentCount : IComponentData {
    [GhostField] public int count;
}

[RequireComponent(typeof(CollidedOpponentAuthoring))]
public class DamageTriggerSource_ShotNonBlockableAuthoring :
    IDamageTriggerSourceAuthoring<DamageTriggerSource.Type.ShotNonBlockable> {
    private class Baker : Baker<DamageTriggerSource_ShotNonBlockableAuthoring> {
        public override void Bake(DamageTriggerSource_ShotNonBlockableAuthoring authoring) {
            var entity = authoring.BakeTriggerSourceBase(this);
            AddComponent<DamagedOpponentCount>(entity);
        }
    }
}