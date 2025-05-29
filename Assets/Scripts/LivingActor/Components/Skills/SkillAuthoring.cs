using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public struct NeedBuildSkillData : IComponentData { }

public class SkillAuthoring : MonoBehaviour {
    public DataSOReader soReader;
    
    private class Baker : TagBaker<SkillAuthoring, NeedBuildSkillData> {
        public override void Bake(SkillAuthoring authoring) {
            base.Bake(authoring);
    
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            if (TryAddChampSkillPrefab(entity, authoring)) return;
        }

        private bool TryAddChampSkillPrefab(in Entity entity, SkillAuthoring authoring) {
            var champTag = authoring.GetComponent<ChampionTagAuthoring>();
            if (champTag == null) return false;
            
            authoring.soReader.Champ![champTag.id].AddAllSkillPrefabBuffer(this, entity);
            return true;
        }
    }
} 