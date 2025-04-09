using NGDtuanh.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SkillPreviewShowerOverride : SkillPreviewShower {
    [SerializeField] private EnumMap<SkillPreviewType, Texture2D> _OverrideTextures;

    private EnumMap<SkillPreviewType, EnumMap<SkillPreviewColor, Material>> _OverrideMaterials;

    protected override EnumMap<SkillPreviewType, EnumMap<SkillPreviewColor, Material>> Materials
        => _OverrideMaterials;

    protected override void Awake() {
        base.Awake();
        
        SkillPreviewDefaultMaterials.BakeMaterials(out _OverrideMaterials, _OverrideTextures);
    }
}