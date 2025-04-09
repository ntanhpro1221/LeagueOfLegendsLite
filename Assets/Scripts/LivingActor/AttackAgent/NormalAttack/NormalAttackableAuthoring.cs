using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct MeleeAttackTrigger : IComponentData, IEnableableComponent { }

[GhostEnabledBit]
public struct RangedAttackTrigger : IComponentData, IEnableableComponent {
    public Entity projectile;
    
    public RangedAttackTrigger(Entity projectile) {
        this.projectile = projectile;
    }
}

[RequireComponent(typeof(AimedTargetAuthoring))]
public class NormalAttackableAuthoring : MonoBehaviour {
    public AttackType attackType;

    [Tooltip("Leave empty if this is a melee attack")]
    public GameObject projectile;

    private class Baker : ExtendBaker<NormalAttackableAuthoring> {
        public override void Bake(NormalAttackableAuthoring authoring) {
            GetDynamicEntity(out var entity);

            switch (authoring.attackType) {
                case AttackType.Melee:
                    AddComponentDisabled<MeleeAttackTrigger>(entity);
                    break;
                case AttackType.Ranged:
                    AddComponentDisabled(entity, new RangedAttackTrigger(GetDynamicEntity(authoring.projectile)));
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum AttackType {
        Melee
      , Ranged
    }
}