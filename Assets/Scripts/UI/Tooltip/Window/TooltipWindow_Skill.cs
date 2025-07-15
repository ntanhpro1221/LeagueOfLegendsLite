using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NGDtuanh.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TooltipWindow_Skill : ITooltipWindow {
    [SerializeField] private Image           _Avatar;
    [SerializeField] private TextMeshProUGUI _Name;
    [SerializeField] private TextMeshProUGUI _Costs;
    [SerializeField] private TextMeshProUGUI _MoreInfoTip;
    [SerializeField] private TextMeshProUGUI _Details;

    private DynamicString        _MainText_Dynamic;
    private DynamicString        _Details_Dynamic;
    private List<float_Q3>       _CooldownTime;
    private List<ItemActiveCost> _ActiveCost;

    private string        _SpecialCondCostFullText;
    private int           _CachedCurrentLevel = -1;
    private int           _MaxLevel;
    private bool          _InDetails;
    private ButtonControl _ShiftBtn;

    private ButtonControl ShiftBtn {
        get {
            if (_ShiftBtn == null) _ShiftBtn = Keyboard.current.shiftKey;
            return _ShiftBtn;
        }
    }

    private void Update() {
        // Show details
        if (ShiftBtn.isPressed != _InDetails) {
            _InDetails = ShiftBtn.isPressed;

            _MoreInfoTip.gameObject.SetActive(!_InDetails);
            _Details.gameObject.SetActive(_InDetails);
        }
    }

    public void Init(IActivableItemSO source) {
        var descriptionDict = new SerializedDictionary<string, List<float_Q3>>(source.GenerateConcreteData_StringKey());

        _Avatar.sprite           = source.avatar;
        _Name.text               = source.itemName;
        _MainText_Dynamic        = new DynamicString(source.description, descriptionDict);
        _Details_Dynamic         = new DynamicString(source.details,     descriptionDict);
        _CooldownTime            = source.cooldownTime;
        _ActiveCost              = source.activeCost;
        _SpecialCondCostFullText = $"\n{source.specialCondCost}".IfOnly(!string.IsNullOrWhiteSpace(source.specialCondCost));
        _MaxLevel                = source.maxLevel;

        UpdateLevel(1);
    }

    /// <param name="newLevel">Start from 1</param>
    public void UpdateLevel(int newLevel) {
        if (newLevel == _CachedCurrentLevel) return;
        _CachedCurrentLevel = newLevel;

        if (_MaxLevel == 0) {
            _MainText.text = _MainText_Dynamic.RawSource;
            _Details.text  = _Details_Dynamic.RawSource;
            _Costs.text    = "";
            return;
        }

        int levelIndex = Mathf.Max(0, newLevel - 1);
        _MainText.text = _MainText_Dynamic.Generate(levelIndex);
        _Details.text  = _Details_Dynamic.Generate(levelIndex);

        var cooldownTime = (int)_CooldownTime[levelIndex];
        var costMana     = (int)_ActiveCost[levelIndex].mana;
        _Costs.text =
            $"{cooldownTime}s Cooldown <sprite name=cooldown>"
          + $"\n{costMana} Mana <sprite name=mana>".IfOnly(costMana > 0)
          + _SpecialCondCostFullText;
    }
}