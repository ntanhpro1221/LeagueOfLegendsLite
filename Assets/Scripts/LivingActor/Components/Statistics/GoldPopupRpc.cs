using Unity.NetCode;

/// <summary>
/// Popup in sender-side
/// </summary>
public struct GoldPopupRpc : IRpcCommand {
    public float_Q3 gold;

    /// <summary>
    /// Not "receiver" because it is going to be shown in sender-side
    /// </summary>
    public SpawnedGhost sender;
}