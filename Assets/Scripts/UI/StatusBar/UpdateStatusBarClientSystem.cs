using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct UpdateStatusBarClientSystem : ISystem {
    private EntityQuery _ownChampQuery;
    private EntityQuery _networkAckQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ClientServerTickRate>();
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<BattleInitData>();
        state.RequireForUpdate<GlobalKDAData>();
        state.RequireForUpdate(_ownChampQuery = SystemAPI.QueryBuilder()
            .WithAll<
                TeamTypeData
              , ChampionTag
              , GhostOwnerIsLocal
              , KDAData
              , CreepScoreData
            >().WithNone<
                DummyTag
            >().Build());
        state.RequireForUpdate(_networkAckQuery = SystemAPI.QueryBuilder()
            .WithAll<
                NetworkSnapshotAck
              , NetworkStreamConnection
            >().Build());
    }

    public void OnUpdate(ref SystemState state) {
        StatusBarUI.Instance.ManualUpdateUI(
            SystemAPI.GetSingleton<GlobalKDAData>().GenerateTextUpdater(
                _ownChampQuery.GetSingleton<TeamTypeData>().team)
          , _ownChampQuery.GetSingleton<KDAData>().GenerateTextUpdater()
          , _ownChampQuery.GetSingleton<CreepScoreData>().GenerateTextUpdater()
          , new TextUpdater.Timer {
                curTick  = SystemAPI.GetSingleton<NetworkTime>().ServerTick
              , tickRate = SystemAPI.GetSingleton<ClientServerTickRate>().SimulationTickRate
            }, new TextUpdater.Ping {
                rtt = _networkAckQuery.GetSingleton<NetworkSnapshotAck>().EstimatedRTT
            });
    }
}