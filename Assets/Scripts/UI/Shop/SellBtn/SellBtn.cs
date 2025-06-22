using System;
using NGDtuanh.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellBtn : MonoBehaviour {
    [SerializeField] private Button          _Button;
    [SerializeField] private TextMeshProUGUI _Text;
    [SerializeField] private Image           _Image;

    [SerializeField] private EnumMap<State, StateData> _StateData;

    private void Awake() {
        ShopUI.Instance.Inspector.SelectedSlot.OnBeforeChanged += (in SlotItemId? oldVal, in SlotItemId? newVal) =>
            UpdateState(newVal);
    }

    private void UpdateState(SlotItemId? selectedItem) {
        _StateData[selectedItem == null
            ? State.NoItemSelected
            : State.CanSell
        ].ApplyTo(this);
    }

    public void OnClick() {
        var item = ShopUI.Instance.Inspector.SelectedSlot.Value;
        if (item == null) {
            Debug.LogError("NGDtuanh click sell when selected item is null (somehow sell button is not disabled when selected item slot changed to null)!");
            return;
        }

        PlayerRequestHub.Instance.SetSellItemAt(item.Value);

        var inspector = ShopUI.Instance.Inspector;
        inspector.SelectedSlot.Value   = null;
        inspector.SelectedItemUI.Value = null;
    }

    public enum State {
        CanSell
      , NoItemSelected
    }

    [Serializable]
    public class StateData {
        [SerializeField] private Color BGColor;
        [SerializeField] private bool  BtnInteractable;
        [SerializeField] private Color BtnTextColor;

        public void ApplyTo(SellBtn purchaseBtn) {
            purchaseBtn._Image.color         = BGColor;
            purchaseBtn._Button.interactable = BtnInteractable;
            purchaseBtn._Text.color          = BtnTextColor;
        }
    }
}