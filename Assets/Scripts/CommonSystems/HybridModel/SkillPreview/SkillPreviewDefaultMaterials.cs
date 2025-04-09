using NGDtuanh.Collections;
using NGDtuanh.Singleton;
using UnityEngine;

public class SkillPreviewDefaultMaterials : SceneSingleton<SkillPreviewDefaultMaterials> {
    public static EnumMap<SkillPreviewType, EnumMap<SkillPreviewColor, Material>> Materials
        => Instance._Materials;

    [SerializeField] private Shader                               _RootShader;
    [SerializeField] private EnumMap<SkillPreviewType, Texture2D> _Textures;
    [SerializeField] private EnumMap<SkillPreviewColor, HDRColor> _Colors;

    private EnumMap<SkillPreviewType, EnumMap<SkillPreviewColor, Material>> _Materials;

    protected override void Awake() {
        base.Awake();

        BakeMaterials(out _Materials);
    }

    public static void BakeMaterials(
        out EnumMap<SkillPreviewType, EnumMap<SkillPreviewColor, Material>> materials
      , EnumMap<SkillPreviewType, Texture2D>                                textures = null
      , EnumMap<SkillPreviewColor, HDRColor>                                colors   = null
      , Shader                                                              shader   = null) {
        textures ??= Instance._Textures;
        colors   ??= Instance._Colors;
        shader   ??= Instance._RootShader;

        foreach (var texKey in (materials = new()).Keys)
        foreach (var colorKey in (materials[texKey] = new()).Keys) {
            var mat = materials[texKey][colorKey] = new Material(shader);

            if (textures[texKey] != null)
                mat.mainTexture  = textures[texKey];
            else mat.mainTexture = Instance._Textures[texKey];

            mat.color = colors[colorKey]; 
        }
    }
}