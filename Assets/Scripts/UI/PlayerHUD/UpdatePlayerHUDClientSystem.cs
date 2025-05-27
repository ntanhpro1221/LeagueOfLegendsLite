using Unity.Entities;
using Unity.NetCode;

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
            stats
          , health
          , mana
          , level
          , prevStateIsDead
          , curDeadState
          , deadData
            )in SystemAPI
            .Query<
                DynamicBuffer<StatsBuffer>
              , RefRO<HealthData>
              , RefRO<ManaData>
              , RefRO<LevelData>
              , EnabledRefRW<PrevStateIsDead>
              , EnabledRefRO<DeadState>
              , RefRO<DeadStateData>
            >().WithAll<
                ChampionTag
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >().WithPresent<
                PrevStateIsDead
              , DeadState>()) {
            // STATS
            playerHUD.Stats.Update(stats, ref index);
            playerHUD.Stats.UpdateCDReduce(333);

            // HEALTH BAR
            playerHUD.HealthBar.UpdateUI(
                maxHealth: stats[index[StatsType.Health]].value
              , curHealth: health.ValueRO.value
              , curArmor: 0
              , maxMana: stats[index[StatsType.Mana]].value
              , curMana: mana.ValueRO.value
              , curLevel: level.ValueRO.curLevel
              , curExp: level.ValueRO.curExp
              , requiredExp: requireExp.CalcRequireExpForNextLevel(level.ValueRO.curLevel));

            // DEAD EVENT
            if (prevStateIsDead.ValueRO != curDeadState.ValueRO) {
                if (curDeadState.ValueRO)
                    playerHUD.DeadHandler.Dead(curTick, deadData.ValueRO.respawnAtTick);
                else playerHUD.DeadHandler.Respawn();
                prevStateIsDead.ValueRW = curDeadState.ValueRO;
            } else if (curDeadState.ValueRO) playerHUD.DeadHandler.UpdateDead(curTick);
        }
    }
}