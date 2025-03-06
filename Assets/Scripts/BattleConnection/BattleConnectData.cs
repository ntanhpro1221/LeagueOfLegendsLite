using Unity.Networking.Transport;
using Unity.Services.Multiplayer;

public struct BattleConnectData {
    public NetworkRole     networkRole;
    public NetworkEndpoint endpoint;
    public TeamType        teamType;

    public readonly BattleInitData ToBattleInitData() => new BattleInitData {
        teamType = teamType
    };
}