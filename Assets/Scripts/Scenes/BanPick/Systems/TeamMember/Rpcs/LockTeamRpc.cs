using Unity.NetCode;

public struct LockTeamRpc : IRpcCommand {
    public TeamType teamId;
}