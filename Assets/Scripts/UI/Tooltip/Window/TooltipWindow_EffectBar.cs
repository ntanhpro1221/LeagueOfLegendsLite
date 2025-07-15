using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TooltipWindow_EffectBar : ITooltipWindow {
    [SerializeField] private Image           _Avatar;
    [SerializeField] private TextMeshProUGUI _Name;
    [SerializeField] private TextMeshProUGUI _Source;

    public void Init(EffectDataManaged data, in FixedString64Bytes sourceName) {
        _Avatar.sprite = data.barData.avatar;
        _Name.text     = data.name;
        _MainText.text = data.description;

        _Source.text = $"Source: {sourceName}";
    }
}