using UnityEngine;

public class DisablableUI_Image_Sprite : DisablableUI_Image<Sprite> {
    protected override Sprite PropSetter { set => Target.sprite = value; }
}