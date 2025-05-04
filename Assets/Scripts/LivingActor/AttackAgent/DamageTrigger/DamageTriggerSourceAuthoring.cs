using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct DamageTriggerSource : IComponentData {
    [GhostField] public float_Q3 damage;
    [GhostField] public Entity   source;

    public DamageTriggerSource(float_Q3 damage, Entity source) {
        this.damage = damage;
        this.source = source;
    }

    public struct TargetedTag : IComponentData { }
    public struct ShotBlockableTag : IComponentData { }
    public struct ShotNonBlockableTag : IComponentData { }
}

public class DamageTriggerSourceAuthoring : MonoBehaviour {
    public float_Q3          damage;
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
                    break;
                case TriggerDamageType.ShotBlockable:
                    AddComponent<DamageTriggerSource.ShotBlockableTag>(entity);

                    AddBuffer<CollidedOpponentBuffer>(entity);
                    AddComponent<TeamTypeData>(entity);
                    break;
                case TriggerDamageType.ShotNonBlockable:
                    AddComponent<DamageTriggerSource.ShotNonBlockableTag>(entity);

                    AddBuffer<CollidedOpponentBuffer>(entity);
                    AddComponent<DamagedOpponentCount>(entity);
                    AddComponent<TeamTypeData>(entity);
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