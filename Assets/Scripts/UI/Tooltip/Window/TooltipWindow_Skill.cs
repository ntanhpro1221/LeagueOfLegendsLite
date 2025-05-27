using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TooltipWindow_Skill : ITooltipWindow {
    [SerializeField] private Image           _Avatar;
    [SerializeField] private TextMeshProUGUI _Name;
    [SerializeField] private TextMeshProUGUI _Costs;
    [SerializeField] private TextMeshProUGUI _MoreInfoTip;
    [SerializeField] private TextMeshProUGUI _Details;

    private DynamicString _MainText_Dynamic;
    private DynamicString _Details_Dynamic;

    private bool          _InDetails;
    private ButtonControl _ShiftBtn;

    private void Awake() {
        _ShiftBtn = Keyboard.current.shiftKey;
    }

    private void Update() {
        if (_ShiftBtn.isPressed != _InDetails) {
            _InDetails = _ShiftBtn.isPressed;

            _MoreInfoTip.gameObject.SetActive(!_InDetails);
            _Details.gameObject.SetActive(_InDetails);
        }
    }

    public void Init(
        Sprite        avatar
      , string        skillName
      , DynamicString mainText_Dynamic
      , DynamicString details_Dynamic) {
        _Avatar.sprite    = avatar;
        _Name.text        = skillName;
        _MainText_Dynamic = mainText_Dynamic;
        _Details_Dynamic  = details_Dynamic;
    }

    public void UpdateCooldown(float cooldown) =>
        _Costs.text = cooldown.ToString(CultureInfo.InvariantCulture);

    public void UpdateLevel(int newLevel) {
        _MainText.text = _MainText_Dynamic.Generate(newLevel);
        _Details.text  = _Details_Dynamic.Generate(newLevel);
    }
}