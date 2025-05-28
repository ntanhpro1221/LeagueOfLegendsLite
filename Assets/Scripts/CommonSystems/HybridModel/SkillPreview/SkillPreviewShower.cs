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

    public void Sync(in SkillPreviewData data) {
        // nothing to update
        if (data.type == prevType && data.color == prevColor) return;
        prevType  = data.type;
        prevColor = data.color;

        // update enable state
        if (meshRenderer.enabled != (data.type != SkillPreviewType.None))
            meshRenderer.enabled = !meshRenderer.enabled;

        // none => do nothing (already disable above)
        if (data.type == SkillPreviewType.None) return;

        // update visual
        meshRenderer.transform.localScale = new(data.scale.x, data.scale.y, 1);
        meshRenderer.material             = Materials[data.type][data.color];
    }
}