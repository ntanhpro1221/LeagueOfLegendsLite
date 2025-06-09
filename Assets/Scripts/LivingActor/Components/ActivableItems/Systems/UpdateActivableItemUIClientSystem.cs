using NGDtuanh.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct UpdateActivableItemUIClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    public void OnUpdate(ref SystemState state) {
        foreach (var (
            itemsStatic
          , itemsDynamic
          , level
            ) in SystemAPI
            .Query<
                RefRO<AllActivableItemData>
              , DynamicBuffer<ActivableItemBonusBuffer>
              , RefRO<LevelData>
            >().WithAll<
                ChampionTag
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >()) {
            UpdateItemCooldown(itemsStatic.ValueRO
              , itemsDynamic
              , SystemAPI.GetSingleton<NetworkTime>().ServerTick
              , PlayerHUD.Instance.ActivableItems.Items, GameSO.TickRate);
            UpdateSkillLevel(itemsStatic.ValueRO
              , itemsDynamic
              , level.ValueRO
              , PlayerHUD.Instance.ActivableItems.Skills, GameSO.TickRate);
        }
    }

    private void UpdateItemCooldown(
        in AllActivableItemData                    itemsStatic
      , in DynamicBuffer<ActivableItemBonusBuffer> itemsDynamic
      , in NetworkTick                             curTick
      , EnumMap<PlayerTrigger.Item, ItemUI>        itemsUI
      , float                                      tickRateFloat) {
        for (PlayerTrigger.Item key = default; key < PlayerTrigger.Item.COUNT; ++key)
            if (itemsStatic.IsActivable(key)) {
                ref var itemStatic  = ref itemsStatic[key];
                var     itemDynamic = itemsDynamic[(int)key];
                var     itemUI      = itemsUI[key];

                bool curInCooldown =
                    itemDynamic.doneAtTick.IsValid
                 && itemDynamic.doneAtTick.IsNewerThan(curTick);

                // Trigger on-off
                if (curInCooldown != itemUI.IsInCooldown) {
                    if (curInCooldown) itemUI.StartCooldown(itemStatic.cooldownTick[itemDynamic.level] / tickRateFloat);
                    else itemUI.DoneCooldown();
                }

                // Update time
                if (curInCooldown)
                    itemUI.UpdateCooldownTime(itemDynamic.doneAtTick.TicksSince(curTick) / tickRateFloat);
            }
    }

    private void UpdateSkillLevel(
        in AllActivableItemData                    itemsStatic
      , in DynamicBuffer<ActivableItemBonusBuffer> itemsDynamic
      , in LevelData                               levelData
      , EnumMap<PlayerTrigger.Item, ItemSkillUI>   itemSkillsUI
      , int                                        tickRate) {
        for (var key = PlayerTrigger.Item.Skill_Q; key <= PlayerTrigger.Item.Skill_R; ++key) {
            var     ui            = itemSkillsUI[key];
            var     tooltipWindow = ui.ItemUI.Tooltip.Window;
            ref var itemStatic    = ref itemsStatic[key];
            var     itemDynamic   = itemsDynamic[(int)key];

            ui.UpdateAll(itemDynamic.level, levelData.availableSkillPoint);
            tooltipWindow.UpdateCooldown(itemStatic.cooldownTick[math.max(0, itemDynamic.level - 1)] * tickRate);
            tooltipWindow.UpdateLevel(itemDynamic.level);
        }
    }
}