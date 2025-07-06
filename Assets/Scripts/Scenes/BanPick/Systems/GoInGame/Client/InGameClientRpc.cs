using Unity.Collections;
using Unity.NetCode;

public struct InGameClientRpc : IRpcCommand {
    public FixedString32Bytes playerName;
}