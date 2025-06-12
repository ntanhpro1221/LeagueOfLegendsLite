using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[GhostEnabledBit]
public struct MeleeAttackTrigger : IComponentData, IEnableableComponent { }

[GhostEnabledBit]
public struct RangedAttackTrigger : IComponentData, IEnableableComponent { }

public struct RangedAttackTriggerData : IComponentData {
    public Entity   projectile;

    public RangedAttackTriggerData(Entity projectile) {
        this.projectile = projectile;
    }
}

[RequireComponent(typeof(AimedTargetAuthoring))]
[RequireComponent(typeof(ProjectileSpawnPointAuthoring))]
public class NormalAttackableAuthoring : MonoBehaviour {
    public AttackType attackType;

    [Tooltip("Leave empty if this is a melee attack")]
    public GameObject projectile;

    public List<DamageTriggerSource.EffectBuffer.Managed> onHitEffects;

    private class Baker : ExtendBaker<NormalAttackableAuthoring> {
        public override void Bake(NormalAttackableAuthoring authoring) {
            GetDynamicEntity(out var entity);

            var onHitEffects = AddBuffer<DamageTriggerSource.EffectBuffer>(entity);
            foreach (var effectManaged in authoring.onHitEffects) onHitEffects.Add(effectManaged.ToUnmanaged());

            switch (authoring.attackType) {
                case AttackType.Melee:
                    AddComponentDisabled<MeleeAttackTrigger>(entity);
                    break;
                case AttackType.Ranged:
                    AddComponentDisabled<RangedAttackTrigger>(entity);
                    AddComponent(entity, new RangedAttackTriggerData(GetDynamicEntity(authoring.projectile)));
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum AttackType {
        Melee  = 0
      , Ranged = 1
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(NormalAttackableAuthoring))]
    private class AuthoringEditor : Editor {
        private SerializedProperty _AttackType;
        private SerializedProperty _Projectile;
        private SerializedProperty _OnHitEffects;

        private void OnEnable() {
            _AttackType   = serializedObject.FindProperty(nameof(attackType));
            _Projectile   = serializedObject.FindProperty(nameof(projectile));
            _OnHitEffects = serializedObject.FindProperty(nameof(onHitEffects));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            var authoring = (NormalAttackableAuthoring)target;

            EditorGUILayout.PropertyField(_AttackType);
            if (authoring.attackType == AttackType.Ranged)
                EditorGUILayout.PropertyField(_Projectile);
            EditorGUILayout.PropertyField(_OnHitEffects);

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}