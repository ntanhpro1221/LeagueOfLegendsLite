using TMPro;
using Unity.Collections;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;

public class EffectBarItemUI : MonoBehaviour {
    [SerializeField] private Image _Avatar;
    [SerializeField] private Image _Outline;
    [SerializeField] private Image _Timer;

    [SerializeField] private RectTransform _TimerOutline;

    [SerializeField] private TextMeshProUGUI _Stack;
    [SerializeField] private Tooltip_EffectBar  _Tooltip;

    public void InitUI(EffectData.Managed data, in FixedString64Bytes sourceName) {
        var barData = data.barData;
        _Avatar.sprite = barData.avatar;
        _Outline.color = barData.outlineColor;
        _Timer.enabled = barData.showTimer;
        _Stack.enabled = barData.showStack;

        _Tooltip.Window.Init(data, sourceName);
    }

    public void UpdateUI(in NetworkTick curTick, in EffectBuffer data) {
        if (_Timer.enabled)
            _TimerOutline.rotation = Quaternion.Euler(0, 0
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
              , 360 * (_Timer.fillAmount = (float)data.endAtTick.TicksSince(curTick) / data.stackTick));

        if (_Stack.enabled)
            _Stack.text = data.curStack.ToString();
    }
}