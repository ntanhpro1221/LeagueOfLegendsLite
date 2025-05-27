using UnityEngine;

public class DisablableUI_Image_Color : DisablableUI_Image<Color> {
    protected override Color PropSetter { set => _Target.color = value; }
}