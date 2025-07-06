using NGDtuanh.Collections;
using NGDtuanh.Singleton;

public class PlayerRequestHub : SceneSingleton<PlayerRequestHub> {
    private readonly EnumMap<InputRequestId, bool> _Event = new();

    public PlayerInputData.RequestData Data;

    private void SetEvent(InputRequestId request) => _Event[request] = true;

    public bool PopEvent(InputRequestId request) {
        bool result = _Event[request];
        _Event[request] = false;
        return result;
    }

    public void SetBuyItem(ItemId _itemToBuy) {
        SetEvent(InputRequestId.BuyItem);
        Data.itemToBuy = _itemToBuy;
    }

    public void SetSellItemAt(SlotItemId _itemSlotToSell) {
        SetEvent(InputRequestId.SellItem);
        Data.itemSlotToSell = _itemSlotToSell;
    }

    public void SetMoveItem(SlotItemId _itemSlotToMove, SlotItemId _itemSlotMoveTarget) {
        SetEvent(InputRequestId.MoveItem);
        Data.itemSlotToMove     = _itemSlotToMove;
        Data.itemSlotMoveTarget = _itemSlotMoveTarget;
    }

    public void SetUpdateSkill(SlotItemId _skillToUpgrade) {
        SetEvent(InputRequestId.UpgradeSkill);
        Data.skillToUpgrade = _skillToUpgrade;
    }
}