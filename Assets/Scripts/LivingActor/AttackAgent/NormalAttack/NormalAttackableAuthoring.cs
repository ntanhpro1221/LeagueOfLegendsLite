using System;
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
    public float_Q3 speed;

    public RangedAttackTriggerData(Entity projectile, float_Q3 speed) {
        this.projectile = projectile;
        this.speed      = speed;
    }
}

[RequireComponent(typeof(AimedTargetAuthoring))]
[RequireComponent(typeof(ProjectileSpawnPointAuthoring))]
public class NormalAttackableAuthoring : MonoBehaviour {
    public AttackType attackType;

    [Tooltip("Leave empty if this is a melee attack")]
    public GameObject projectile;

    public float_Q3 speed;

    private class Baker : ExtendBaker<NormalAttackableAuthoring> {
        public override void Bake(NormalAttackableAuthoring authoring) {
            GetDynamicEntity(out var entity);

            switch (authoring.attackType) {
                case AttackType.Melee:
                    AddComponentDisabled<MeleeAttackTrigger>(entity);
                    break;
                case AttackType.Ranged:
                    AddComponentDisabled<RangedAttackTrigger>(entity);
                    AddComponent(entity, new RangedAttackTriggerData(
                        GetDynamicEntity(authoring.projectile)
                      , authoring.speed));
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
        private SerializedProperty _Speed;

        private void OnEnable() {
            _AttackType = serializedObject.FindProperty(nameof(attackType));
            _Projectile = serializedObject.FindProperty(nameof(projectile));
            _Speed      = serializedObject.FindProperty(nameof(speed));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            var authoring = (NormalAttackableAuthoring)target;

            EditorGUILayout.PropertyField(_AttackType);
            if (authoring.attackType == AttackType.Ranged) {
                EditorGUILayout.PropertyField(_Projectile);
                EditorGUILayout.PropertyField(_Speed);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}