using System;
using NGDtuanh.Collections;
using NGDtuanh.Utils;
using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleActivableItemDataSystemGroup))]
public partial struct ModifyItemSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllItemData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            allItem = SystemAPI.GetSingleton<AllItemData>()
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public AllItemData allItem;

        [BurstCompile]
        public void Execute(
            ref GoldData           gold
          , ref ItemSlotsData      itemSlots
          , ref StatBuffs.Receiver buffsReceiver
          , PlayerInputAspectRO    input) {
            TryMoveItem(ref itemSlots, input);

            TrySellItem(ref gold, ref itemSlots, ref buffsReceiver, input);

            TryBuyItem(ref gold, ref itemSlots, ref buffsReceiver, input);

            RehashItems(ref itemSlots);
        }

        #region MAIN FUNCS

        private static void TryMoveItem(
            ref ItemSlotsData   itemSlots
          , PlayerInputAspectRO input) {
            // DO NOTHING WHEN: NOT HAVE REQUEST :V
            if (!input.GetEvent_WithData(InputRequestId.MoveItem)) return;

            var fromSlot = input.Input.requestData.itemSlotToMove;
            if (!ValidateSlot(fromSlot, itemSlots, requireContainItem: true)) return;
            ref var fromSlotData = ref itemSlots.data.ValueRW(fromSlot);

            var toSlot = input.Input.requestData.itemSlotMoveTarget;
            if (!ValidateSlot(toSlot, itemSlots, requireContainItem: false)) return;
            ref var toSlotData = ref itemSlots.data.ValueRW(toSlot);

            // MOVE ITEM 
            if (!toSlotData.common.containItem) {
                // set target slot
                toSlotData = fromSlotData;
                // empty from slot
                fromSlotData.RemoveItem();
            } else Swapper.Swap(ref toSlotData, ref fromSlotData);
        }

        private void TrySellItem(
            ref GoldData           gold
          , ref ItemSlotsData      itemSlots
          , ref StatBuffs.Receiver buffsReceiver
          , PlayerInputAspectRO    input) {
            // DO NOTHING WHEN: NOT HAVE REQUEST :V
            if (!input.GetEvent_WithData(InputRequestId.SellItem)) return;

            var sellSlot = input.Input.requestData.itemSlotToSell;
            if (!ValidateSlot(sellSlot, itemSlots, requireContainItem: true)) return;
            ref var sellSlotData = ref itemSlots.data.ValueRW(sellSlot);

            // SELL ITEM
            // refund
            gold.gold += allItem.Items[sellSlotData.itemId].settings.sell;
            // remove item
            RemoveItemAt(ref sellSlotData, ref buffsReceiver);
        }

        private void TryBuyItem(
            ref GoldData           gold
          , ref ItemSlotsData      itemSlots
          , ref StatBuffs.Receiver buffsReceiver
          , PlayerInputAspectRO    input) {
            // DO NOTHING WHEN: NOT HAVE REQUEST :V
            if (!input.GetEvent_WithData(InputRequestId.BuyItem)) return;
            var     buyItem     = input.Input.requestData.itemToBuy;
            ref var buyItemData = ref allItem.Items[buyItem];

            Strum.SlotItem.Fields<bool> sacrificeSlots = default;
            FindSacrificeSlots(ref sacrificeSlots, buyItem, itemSlots, ref allItem);
            var requiredGold = buyItemData.settings.cost;
            var targetSlot   = (SlotItemId)(-1);
            for (var slot = Strum.SlotItem.Last_Item; slot >= Strum.SlotItem.First_Item; --slot)
                if (sacrificeSlots[slot]) {
                    requiredGold -= allItem.Items[itemSlots.data[slot].itemId].settings.cost;
                    targetSlot   =  slot;
                } else if (!itemSlots.data[slot].common.containItem) targetSlot = slot;

            // DO NOTHING WHEN: NOT ENOUGH SPACE
            if (!targetSlot.IsItem()) return;

            // DO NOTHING WHEN: NOT ENOUGH MONEY
            if (gold.gold < requiredGold) return;

            // BUY ITEM
            // apply cost
            gold.gold -= requiredGold;
            // sacrifice item            
            for (var slot = Strum.SlotItem.First_Item; slot <= Strum.SlotItem.Last_Item; ++slot)
                if (sacrificeSlots[slot])
                    RemoveItemAt(ref itemSlots.data.ValueRW(slot), ref buffsReceiver);
            // add new item
            AddItemAt(ref itemSlots.data.ValueRW(targetSlot), ref buffsReceiver, buyItem);
        }

        private static void RehashItems(ref ItemSlotsData itemSlots) {
            ref var hash = ref itemSlots.serverHash;
            hash = 0;
            for (var slot = Strum.SlotItem.First_Item; slot <= Strum.SlotItem.Last_Item; ++slot)
                if (itemSlots.data[slot].common.containItem)
                    // Hash with slotId and itemId at that slot
                    hash = HashCode.Combine(hash, (int)slot, (int)itemSlots.data[slot].itemId);
        }

        #endregion

        #region UTILS

        private static bool ValidateSlot(SlotItemId slotId, in ItemSlotsData itemSlots, bool requireContainItem) =>
            slotId.IsItem()
         && (!requireContainItem || itemSlots.data[slotId].common.containItem);

        private void AddItemAt(
            ref ItemSlotsData.Element slotData
          , ref StatBuffs.Receiver    buffsReceiver
          , ItemId                    item) {
            buffsReceiver.Add(ref allItem.Items[item].buffs);
            slotData.SetItem(item);
        }

        private void RemoveItemAt(
            ref ItemSlotsData.Element slotData
          , ref StatBuffs.Receiver    buffsReceiver) {
            buffsReceiver.Remove(ref allItem.Items[slotData.itemId].buffs);
            slotData.RemoveItem();
        }

        #endregion
    }

    public static void FindSacrificeSlots(
        ref Strum.SlotItem.Fields<bool> sacrificeSlots
      , ItemId                          targetItem
      , in  ItemSlotsData               itemSlots
      , ref AllItemData                 allItem) {
        ref var targetRecipe = ref allItem.Items[targetItem].recipe;
        for (int i = 0; i < targetRecipe.Count; ++i)
        for (var slot = Strum.SlotItem.First_Item; slot <= Strum.SlotItem.Last_Item; ++slot)
            if ( // Slot has not sacrificed yet
                !sacrificeSlots[slot]
                // Slot contains item
             && itemSlots.data[slot].common.containItem
                // Slot's item is suitable
             && targetRecipe[i] == itemSlots.data[slot].itemId) {
                sacrificeSlots[slot] = true;
                break;
            } else FindSacrificeSlots(ref sacrificeSlots, targetRecipe[i], itemSlots, ref allItem);
    }

    public static void FindSacrificeSlots(
        ref Strum.SlotItem.Fields<bool>     sacrificeSlots
      , ItemId                              targetItem
      , EnumMap<SlotItemId, IItemUIWrapper> itemUIs) {
        foreach (var child in GameSO.Item[targetItem].recipe)
            for (var slot = Strum.SlotItem.First_Item; slot <= Strum.SlotItem.Last_Item; ++slot)
                if ( // Slot has not sacrificed yet
                    !sacrificeSlots[slot]
                    // Slot contains item
                 && ((ItemUI)itemUIs[slot]).Core.gameObject.activeSelf
                    // Slot's item is suitable
                 && child == ((ItemUI)itemUIs[slot]).CurItem) {
                    sacrificeSlots[slot] = true;
                    break;
                } else FindSacrificeSlots(ref sacrificeSlots, child, itemUIs);
    }
}