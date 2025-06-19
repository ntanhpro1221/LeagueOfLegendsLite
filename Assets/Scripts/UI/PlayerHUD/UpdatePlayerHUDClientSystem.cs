using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct UpdatePlayerHUDClientSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<RequireExpData>();
        state.RequireForUpdate<NetworkTime>();
    }

    public void OnUpdate(ref SystemState state) {
        var playerHUD  = PlayerHUD.Instance;
        var requireExp = SystemAPI.GetSingleton<RequireExpData>();
        var curTick    = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

        foreach (var (
            curDeadState
          , deadData
          , healthBarUpdateGenerator
          , deadTrigger
          , gold
          , cc
            ) in SystemAPI
            .Query<
                EnabledRefRO<DeadState>
              , RefRO<DeadStateData>
              , HealthBarUpdateAspect
              , DeadTriggerForUIData
              , RefRO<GoldData>
              , CCAspectRO
            >().WithAll<
                ChampionTag
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >().WithPresent<
                DeadState
            >()) {
            // STATS
            playerHUD.Stats.Update(healthBarUpdateGenerator.Stats);
            playerHUD.Stats.UpdateCDReduce(333);

            // HEALTH BAR
            playerHUD.HealthBar.UpdateUI(healthBarUpdateGenerator.GenerateUpdateData(requireExp));

            // DEAD EVENT
            deadTrigger.UpdateHandler(
                playerHUD.DeadHandler
              , curTick
              , deadData.ValueRO
              , curDeadState);

            // GOLD
            playerHUD.UpdateGold((int)gold.ValueRO.gold);

            // EXP TOOLTIP
            playerHUD.UpdateExp(healthBarUpdateGenerator.Level, requireExp);
            
            // BLOCK ACTIVABLE ITEM
            var blockItemsNow = cc.Disable.ActiveItem != 0;
            if (playerHUD.ActivableItems.BlockedAllItems != blockItemsNow) {
                if (blockItemsNow) playerHUD.ActivableItems.StartBlockAllItems();
                else playerHUD.ActivableItems.DoneBlockAllItems();
            }
        }
    }
}