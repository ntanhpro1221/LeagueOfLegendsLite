using UnityEngine;

public class DisablableUI_Image_Material : DisablableUI_Image<Material> {
    protected override Material PropSetter { set => Target.material = value; }
}