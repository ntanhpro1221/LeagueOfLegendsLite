using UnityEngine;

public class DisablableUI_Text_Color : DisablableUI_Text<Color> {
    protected override Color PropSetter { set => _Target.color = value; }
}