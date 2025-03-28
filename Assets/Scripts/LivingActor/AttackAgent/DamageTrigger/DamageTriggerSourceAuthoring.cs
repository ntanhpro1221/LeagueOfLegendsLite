using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct DamageTriggerSource : IComponentData {
    [GhostField] public int damage;

    public struct TargetedTag : IComponentData { }
    public struct ShotBlockableTag : IComponentData { }
    public struct ShotNonBlockableTag : IComponentData { }
}

[RequireComponent(typeof(TeamTypeAuthoring))]
public class DamageTriggerSourceAuthoring : MonoBehaviour {
    public int               damage;
    public TriggerDamageType damageType;

    private class Baker : Baker<DamageTriggerSourceAuthoring> {
        public override void Bake(DamageTriggerSourceAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DamageTriggerSource {
                damage = authoring.damage
            });

            switch (authoring.damageType) {
                case TriggerDamageType.Targeted:
                    AddComponent<DamageTriggerSource.TargetedTag>(entity);

                    AddComponent<DamageTargetData>(entity);
                    break;
                case TriggerDamageType.ShotBlockable:
                    AddComponent<DamageTriggerSource.ShotBlockableTag>(entity);

                    AddBuffer<CollidedOpponentBuffer>(entity);
                    break;
                case TriggerDamageType.ShotNonBlockable:
                    AddComponent<DamageTriggerSource.ShotNonBlockableTag>(entity);

                    AddBuffer<CollidedOpponentBuffer>(entity);
                    AddComponent<DamagedOpponentCount>(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum TriggerDamageType {
        Targeted
      , ShotBlockable
      , ShotNonBlockable
    }
}