using UnityEngine;

public class ItemUIBuildInto : IItemUIWrapper {
    [SerializeField] private Tooltip_Item _Tooltip;

    private ItemId _CurItem;

    public void InitAll(ItemId itemId) {
        var data = GameSO.Item[itemId];

        // Init myself
        _CurItem = itemId;

        // Init core
        Core.Avatar.sprite = data.common.avatar;

        // Init tooltip
        _Tooltip.Window.Init(data);
    }

    public void OnClick() {
        ShopUI.Instance.Inspector.InspectItem(_CurItem, null, true);
    }
}