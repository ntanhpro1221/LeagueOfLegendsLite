using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct UpdatePlayerHUDClientSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<RequireExpData>();
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate<NetworkTime>();
    }

    public void OnUpdate(ref SystemState state) {
        var     playerHUD  = PlayerHUD.Instance;
        var     requireExp = SystemAPI.GetSingleton<RequireExpData>();
        var     curTick    = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
        ref var index      = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;

        foreach (var (
            curDeadState
          , deadData
          , healthBarUpdateGenerator
          , deadTrigger
            ) in SystemAPI
            .Query<
                EnabledRefRO<DeadState>
              , RefRO<DeadStateData>
              , HealthBarUpdateAspect
              , DeadTriggerForUIData
            >().WithAll<
                ChampionTag
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >().WithPresent<DeadState>()) {
            // STATS
            playerHUD.Stats.Update(healthBarUpdateGenerator.Stats, ref index);
            playerHUD.Stats.UpdateCDReduce(333); 

            // HEALTH BAR
            playerHUD.HealthBar.UpdateUI(healthBarUpdateGenerator.GenerateUpdateData(
                index[StatsType.Health]
              , index[StatsType.Mana]
              , requireExp));

            // DEAD EVENT
            deadTrigger.UpdateHandler(
                playerHUD.DeadHandler
              , curTick
              , deadData.ValueRO
              , curDeadState);
        }
    }
}