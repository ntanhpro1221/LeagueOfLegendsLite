using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : IItemUIWrapper, ISelectable, IDragItem, IFilteredDropHandler<ItemUI>, IPointerClickHandler {
    [field: SerializeField] public  Tooltip_Item Tooltip { get; private set; }
    [SerializeField]        private Image        _ShopSelectedImg;

    [Header("--------DRAG HOVER--------")]
    [SerializeField] private Image _DragHoverImg;

    [SerializeField] private Color _DragHoverNormalColor;
    [SerializeField] private Color _DragHoverHighlightColor;

    public SlotItemId MySlot  { get; private set; }
    public ItemId     CurItem { get; private set; }

    public void InitAll(ItemId itemId) {
        var data = GameSO.Item[itemId];

        // Init myself
        CurItem = itemId;

        // Item's avatar
        Core.Avatar.sprite = data.common.avatar;

        // Tooltip window
        Tooltip.Window.Init(data);
    }

    public void SetSlot(SlotItemId slot) => MySlot = slot;

    #region SELECT ITEM

    public void Select()   => _ShopSelectedImg.gameObject.SetActive(true);
    public void Deselect() => _ShopSelectedImg.gameObject.SetActive(false);

    #endregion

    #region DRAG ITEM

    private void SetDragComponent(bool active) =>
        Core.ActiveKeyText.enabled
            = Core.StackText.enabled
                = Core.Border.enabled
                    = _ShopSelectedImg.enabled
                        = !active;

    public void OnBeginDrag(PointerEventData eventData) => SetDragComponent(true);

    public void OnEndDrag(PointerEventData eventData) => SetDragComponent(false);

    #endregion

    #region DROP ITEM

    public void OnItemDrop(ItemUI target) {
        PlayerRequestHub.Instance.SetMoveItem(target.MySlot, MySlot);

        var inspector = ShopUI.Instance.Inspector;
        if (inspector.SelectedItemUI.Equals(this) || inspector.SelectedItemUI.Equals(target)) {
            inspector.SelectedSlot.Value   = null;
            inspector.SelectedItemUI.Value = null;
        }
    }

    public void OnItemEnter(ItemUI item) => _DragHoverImg.color = _DragHoverHighlightColor;

    public void OnItemExit(ItemUI item) => _DragHoverImg.color = _DragHoverNormalColor;

    public void OnItemBeginDrag(ItemUI target) => _DragHoverImg.enabled = true;

    public void OnItemEndDrag(ItemUI item) => (_DragHoverImg.enabled, _DragHoverImg.color) = (false, _DragHoverNormalColor);

    #endregion

    #region CLICK HANDLE

    public void OnPointerClick(PointerEventData eventData) {
        if (!Core.gameObject.activeSelf) return;

        // Right mouse is sell item
        if (eventData.button == PointerEventData.InputButton.Right) {
            PlayerRequestHub.Instance.SetSellItemAt(MySlot);
            
            var inspector = ShopUI.Instance.Inspector;
            if (inspector.SelectedItemUI.Equals(this)) {
                inspector.SelectedSlot.Value   = null;
                inspector.SelectedItemUI.Value = null;
            }

            // Other mouse btn is inspecting
        } else ShopUI.Instance.Inspector.InspectItem(CurItem, this, true, MySlot);
    }

    #endregion
}