using Unity.NetCode;

public struct ExpPopupRpc : IRpcCommand {
    public int          exp;
    public SpawnedGhost receiver;
}