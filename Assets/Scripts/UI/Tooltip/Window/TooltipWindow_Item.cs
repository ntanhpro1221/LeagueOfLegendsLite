using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TooltipWindow_Item : ITooltipWindow {
    [SerializeField] private Image           _Avatar;
    [SerializeField] private TextMeshProUGUI _Name;
    [SerializeField] private TextMeshProUGUI _CostSell;
    [SerializeField] private TextMeshProUGUI _MoreInfoTip;
    [SerializeField] private TextMeshProUGUI _Details;

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

    public void Init(ItemDataManaged source) => Init(
        avatar: source.common.avatar
      , itemName: source.common.itemName
      , description: source.common.description
      , details: source.common.details
      , cost: source.settings.cost
      , sell: source.settings.sell);

    public void Init(
        Sprite   avatar
      , string   itemName
      , string   description
      , string   details
      , float_Q3 cost
      , float_Q3 sell) {
        _Avatar.sprite = avatar;
        _Name.text     = itemName;
        _MainText.text = description;
        _Details.text  = details;
        _CostSell.text =
            $"Cost: <sprite name=coin> {cost}"
          + $"\nSell: <sprite name=coin> {sell} ({sell / cost * 100:int}%)";
    }
}