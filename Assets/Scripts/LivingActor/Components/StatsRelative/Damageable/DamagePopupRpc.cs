using Unity.NetCode;

public struct DamagePopupRpc : IRpcCommand {
    public float_Q3     damage;
    public SpawnedGhost receiver;
}