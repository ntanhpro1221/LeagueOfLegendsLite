using System;
using NGDtuanh.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUIShop : IItemUIWrapper, ISelectable, IPointerClickHandler {
    [SerializeField] private bool _UpdateRecipeOnSelected;

    [SerializeField] private Tooltip_Item    _Tooltip;
    [SerializeField] private Image           _SelectedBorder;
    [SerializeField] private TextMeshProUGUI _CostText;
    [SerializeField] private Image           _Border;
    [SerializeField] private Image           _PurchasedMark;

    public ItemId CurItem { get; private set; }

    public void InitAll(ItemId itemId, ItemDataManaged data) {
        // Init myself
        CurItem = itemId;
        SetCost(data.settings.cost);

        // Init core
        Core.Avatar.sprite = data.common.avatar;

        // Init tooltip
        _Tooltip.Window.Init(data);
    }

    public void SetCost(in float_Q3 cost) => _CostText.text = $"{cost:int}";

    #region SELECT ITEM

    public void Select() {
        _SelectedBorder.gameObject.SetActive(true);
    }

    public void Deselect() {
        _SelectedBorder.gameObject.SetActive(false);
    }

    #endregion

    #region STATE HANDLE

    [SerializeField] private EnumMap<State, StateData> _StateData;

    public bool Purchased { get; private set; }

    public void UpdateState(bool buyable, bool purchased = false) => _StateData[
        // ReSharper disable once AssignmentInConditionalExpression
        (Purchased = purchased)
            ? State.Purchased
            : buyable
                ? State.Buyable
                : State.NotEnoughGold
    ].ApplyTo(this);

    public enum State {
        Purchased
      , Buyable
      , NotEnoughGold
    }

    [Serializable]
    public class StateData {
        [SerializeField] private Color AvatarColor;
        [SerializeField] private Color BorderColor;
        [SerializeField] private Color CostColor;
        [SerializeField] private bool  Purchased;

        public void ApplyTo(ItemUIShop target) {
            target.Core.Avatar.color      = AvatarColor;
            target._Border.color          = BorderColor;
            target._CostText.color        = CostColor;
            target._PurchasedMark.enabled = Purchased;
        }
    }

    #endregion

    #region CLICK HANDLE

    public void OnPointerClick(PointerEventData eventData) {
        // Right mouse is buy item
        if (eventData.button == PointerEventData.InputButton.Right)
            PlayerRequestHub.Instance.SetBuyItem(CurItem);
        // Other mouse btn is inspecting
        else ShopUI.Instance.Inspector.InspectItem(CurItem, this, _UpdateRecipeOnSelected);
    }

    #endregion
}