using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct UpdateStatusBarClientSystem : ISystem {
    private EntityQuery ownChampQuery;
    private EntityQuery networkAckQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ClientServerTickRate>();
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<BattleClientData>();
        state.RequireForUpdate<GlobalKDAData>();
        state.RequireForUpdate(networkAckQuery = SystemAPI.QueryBuilder()
            .WithAll<
                NetworkSnapshotAck
              , NetworkStreamConnection
            >().Build());
        
        ownChampQuery = SystemAPI.QueryBuilder()
            .WithAll<
                TeamTypeData
              , ChampionTag
              , GhostOwnerIsLocal
              , KDAData
              , CreepScoreData
            >().WithNone<
                DummyTag
            >().Build();
    }

    public void OnUpdate(ref SystemState state) {
        if (!StatusBarUI.IsAvailable) return;

        if (ownChampQuery.IsEmpty) return;

        StatusBarUI.Instance.ManualUpdateUI(
            SystemAPI.GetSingleton<GlobalKDAData>().GenerateTextUpdater(
                ownChampQuery.GetSingleton<TeamTypeData>().team)
          , ownChampQuery.GetSingleton<KDAData>().GenerateTextUpdater()
          , ownChampQuery.GetSingleton<CreepScoreData>().GenerateTextUpdater()
          , new TextUpdater.Timer {
                curTick  = SystemAPI.GetSingleton<NetworkTime>().ServerTick
              , tickRate = SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate
            }, new TextUpdater.Ping {
                rtt = networkAckQuery.GetSingleton<NetworkSnapshotAck>().EstimatedRTT
            });
    }
}