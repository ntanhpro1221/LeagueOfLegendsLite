using NGDtuanh.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(HandleItemClientUISystemGroup))]
public partial struct UpdateActivableItemsUI_OwnChamp_ClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllItemData>();
        state.RequireForUpdate<NetworkTime>();
    }

    public void OnUpdate(ref SystemState state) {
        var allItem = SystemAPI.GetSingleton<AllItemData>();

        foreach (var (
            itemSlots
          , level
          , hybrid
            ) in SystemAPI
            .Query<
                ItemSlotsAspectRO
              , RefRO<LevelData>
              , RefRO<HybridModelData>
            >().WithAll<
                ChampionTag
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >()) {
            UpdateModifiedItem(itemSlots
              , allItem
              , PlayerHUD.Instance.ActivableItems.Items
              , hybrid.ValueRO.indicator);
            UpdateAllItemCooldown(itemSlots
              , allItem
              , SystemAPI.GetSingleton<NetworkTime>().ServerTick
              , PlayerHUD.Instance.ActivableItems.Items, GameSO.TickRate);
            UpdateSkillLevel(itemSlots
              , allItem
              , level.ValueRO
              , PlayerHUD.Instance.ActivableItems.Items);
        }
    }

    private void UpdateModifiedItem(
        in ItemSlotsAspectRO                itemSlots
      , in AllItemData                      allItem
      , EnumMap<SlotItemId, IItemUIWrapper> itemsUI
      , IndicatorShower                     indicatorShower) {
        if (!itemSlots.RawSlots.NeedFix) return;

        for (var slot = Strum.SlotItem.First_Item; slot <= Strum.SlotItem.Last_Item; ++slot) {
            var itemDynamic = itemSlots.Slots[slot];
            var itemUI      = (ItemUI)itemsUI[slot];

            // Update visible
            bool containItem = itemDynamic.common.containItem;
            itemUI.Core.gameObject.SetActive(containItem);

            if (!containItem) continue;
            var itemManaged = GameSO.Item[itemDynamic.itemId];

            // Update info
            itemUI.InitAll(itemDynamic.itemId);

            // Update indicator
            indicatorShower.UpdateIndicatorAt(slot, itemManaged.indicator);
        }

        // Event must be post after applied changes to UI.
        LazyObserver_Battle.PostEvent(LazyObserver_Battle.Events.SlotChanged);
    }

    private void UpdateAllItemCooldown(
        in ItemSlotsAspectRO                itemSlots
      , in AllItemData                      allItem
      , in NetworkTick                      curTick
      , EnumMap<SlotItemId, IItemUIWrapper> itemsUI
      , float                               tickRateFloat) {
        foreach (var slot in Strum.SlotItem.Indexes)
            if (itemSlots.IsActivable(slot, allItem)) {
                ref var itemStatic  = ref itemSlots.GetItemDataUnsafe(slot, allItem);
                var     itemDynamic = itemSlots.Slots[slot];
                var     itemUICore  = itemsUI[slot].Core;
                int     levelIndex  = itemStatic.CalcLevelIndex(itemDynamic.level);

                bool curInCooldown =
                    itemDynamic.common.doneAtTick.IsValid
                 && itemDynamic.common.doneAtTick.IsNewerThan(curTick);

                // Trigger on-off
                if (curInCooldown != itemUICore.IsInCooldown) {
                    if (curInCooldown) itemUICore.StartCooldown(itemStatic.cooldownTick[levelIndex] / tickRateFloat);
                    else itemUICore.DoneCooldown();
                }

                // Update time
                if (curInCooldown)
                    itemUICore.UpdateCooldownTime(itemDynamic.common.doneAtTick.TicksSince(curTick) / tickRateFloat);
            }
    }

    private void UpdateSkillLevel(
        in ItemSlotsAspectRO                itemSlots
      , in AllItemData                      allItem
      , in LevelData                        levelData
      , EnumMap<SlotItemId, IItemUIWrapper> itemSkillsUI) {
        for (var slot = Strum.SlotItem.First_SkillNotPassive; slot <= Strum.SlotItem.Last_Skill; ++slot) {
            if (!itemSlots.Slots[slot].common.containItem) continue;

            var ui            = (ItemSkillUI)itemSkillsUI[slot];
            var tooltipWindow = ui.Tooltip.Window;
            var itemDynamic   = itemSlots.Slots[slot];

            ui.UpdateAll(itemDynamic.level, levelData.availableSkillPoint);
            tooltipWindow.UpdateLevel(itemDynamic.level);
        }
    }
}