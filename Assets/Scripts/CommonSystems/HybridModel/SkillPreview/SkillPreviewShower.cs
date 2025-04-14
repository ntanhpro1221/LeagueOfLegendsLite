using NGDtuanh.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SkillPreviewShower : MonoBehaviour {
    protected virtual EnumMap<SkillPreviewType, EnumMap<SkillPreviewColor, Material>> Materials
        => SkillPreviewDefaultMaterials.Materials;

    private MeshRenderer      meshRenderer;
    private SkillPreviewType  prevType;
    private SkillPreviewColor prevColor;

    protected virtual void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Sync(SkillPreviewType type, SkillPreviewColor color, Vector2 scale) {
        // nothing to update
        if (type == prevType && color == prevColor) return;
        prevType  = type;
        prevColor = color;

        // update enable state
        if (meshRenderer.enabled != (type != SkillPreviewType.None))
            meshRenderer.enabled = !meshRenderer.enabled;

        // none => do nothing (already disable above)
        if (type == SkillPreviewType.None) return;

        // update visual
        meshRenderer.transform.localScale = new(scale.x, scale.y, 1);
        meshRenderer.material             = Materials[type][color];
    }
}