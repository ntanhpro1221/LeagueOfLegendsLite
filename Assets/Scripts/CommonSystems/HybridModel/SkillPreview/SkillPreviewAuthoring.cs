using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct SkillPreviewData : IComponentData {
    public SkillPreviewType  type;
    public SkillPreviewColor color;
    public float2            scale;
}

public class SkillPreviewAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<SkillPreviewAuthoring> {
        public override void Bake(SkillPreviewAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<SkillPreviewData>(entity);
        }
    }
}