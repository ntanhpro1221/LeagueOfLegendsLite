using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct UpdateActivableItemUISystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    public void OnUpdate(ref SystemState state) {
        UpdateSkillCooldown(ref state, SystemAPI.GetSingleton<NetworkTime>().ServerTick);
    }

    public void UpdateSkillCooldown(ref SystemState state, NetworkTick curTick) {
        var   items         = PlayerHUD.Instance.ActivableItems.Items;
        float tickRateFloat = GameSO.TickRate;
    
        foreach (var (
            itemsStatic
          , itemsDynamic
            ) in SystemAPI
            .Query<
                RefRO<AllActivableItemData>
              , DynamicBuffer<ActivableItemBonusBuffer>
            >().WithAll<
                ChampionTag
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >())
            for (PlayerTrigger.Item key = default; key < PlayerTrigger.Item.COUNT; ++key)
                if (itemsStatic.ValueRO.IsActivable(key)) {
                    ref var itemStatic  = ref itemsStatic.ValueRO[key];
                    var     itemDynamic = itemsDynamic[(int)key];
                    var     itemUI      = items[key];

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
}