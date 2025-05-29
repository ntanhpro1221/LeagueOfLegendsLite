using System.Collections.Generic;
using System.Globalization;
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

    private DynamicString               _MainText_Dynamic;
    private DynamicString               _Details_Dynamic;
    private List<ItemCommonLeveledData> _LeveledData_Common;

    private int           _MaxLevel;
    private bool          _InDetails;
    private ButtonControl _ShiftBtn;

    private void Awake() {
        _ShiftBtn = Keyboard.current.shiftKey;
    }

    private void Update() {
        // Show details
        if (_ShiftBtn.isPressed != _InDetails) {
            _InDetails = _ShiftBtn.isPressed;

            _MoreInfoTip.gameObject.SetActive(!_InDetails);
            _Details.gameObject.SetActive(_InDetails);
        }
    }

    public void Init(
        Sprite                      avatar
      , string                      skillName
      , DynamicString               mainText_Dynamic
      , DynamicString               details_Dynamic
      , List<ItemCommonLeveledData> leveledData_Common
      , int                         maxLevel) {
        _Avatar.sprite      = avatar;
        _Name.text          = skillName;
        _MainText_Dynamic   = mainText_Dynamic;
        _Details_Dynamic    = details_Dynamic;
        _LeveledData_Common = leveledData_Common;
        _MaxLevel           = maxLevel;

        UpdateLevel(1);
    }

    public void UpdateCooldown(float cooldown) =>
        _Costs.text = cooldown.ToString(CultureInfo.InvariantCulture);

    /// <param name="newLevel">Start from 1</param>
    public void UpdateLevel(int newLevel) {
        if (_MaxLevel == 0) {
            _MainText.text = _MainText_Dynamic.RawSource;
            _Details.text = _Details_Dynamic.RawSource;
            _Costs.text = "";
            return; 
        }

        --newLevel;
        _MainText.text = _MainText_Dynamic.Generate(newLevel);
        _Details.text  = _Details_Dynamic.Generate(newLevel);
        CostsUpdate(newLevel);
    }

    private void CostsUpdate(int newLevel) {
        var data = _LeveledData_Common[newLevel];
        _Costs.text =
            $"{data.cooldownTime}s Cooldown"
          + $"\n{data.cost.mana} Mana";
    }
}