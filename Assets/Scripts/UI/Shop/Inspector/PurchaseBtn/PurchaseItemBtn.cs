using System;
using NGDtuanh.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseItemBtn : MonoBehaviour {
    [SerializeField] private Button          _Button;
    [SerializeField] private TextMeshProUGUI _Text;
    [SerializeField] private Image           _Image;

    [SerializeField] private EnumMap<State, StateData> _StateData;

    private void Awake() {
        var inspector = ShopUI.Instance.Inspector;
        var buyable   = ShopUI.Instance.Buyable;

        inspector.SelectedItem.OnBeforeChanged += (in ItemId? oldVal, in ItemId? newVal) => UpdateState(newVal, buyable.Value);

        buyable.OnBeforeChanged += (in Strum.Items.Fields<bool> oldVal, in Strum.Items.Fields<bool> newVal) => UpdateState(inspector.SelectedItem, newVal);
    }

    private void UpdateState(ItemId? selectedItem, in Strum.Items.Fields<bool> buyable) {
        if (selectedItem == null) {
            Debug.LogWarning("NGDtuanh PurchaseItemBtn: somehow selected item is null! (If this only happen when deselected item or buyable changed, this is normal)");
            return;
        }

        _StateData[buyable[selectedItem.Value]
            ? State.CanBuy
            : State.NotEnoughGold
        ].ApplyTo(this);
    }

    public void OnClick() {
        var item = ShopUI.Instance.Inspector.SelectedItem.Value;
        if (item == null) {
            Debug.LogError("NGDtuanh PurchaseItemBtn: click purchase when selected item is null (somehow purchase button is not disabled when selected item changed to null)!");
            return;
        }

        PlayerRequestHub.Instance.SetBuyItem(item.Value);
    }

    public enum State {
        CanBuy
      , NotEnoughGold
    }

    [Serializable]
    public class StateData {
        [SerializeField] private Color  BGColor;
        [SerializeField] private bool   BtnInteractable;
        [SerializeField] private string BtnText;
        [SerializeField] private Color  BtnTextColor;

        public void ApplyTo(PurchaseItemBtn purchaseBtn) {
            purchaseBtn._Image.color         = BGColor;
            purchaseBtn._Button.interactable = BtnInteractable;
            purchaseBtn._Text.text           = BtnText;
            purchaseBtn._Text.color          = BtnTextColor;
        }
    }
}