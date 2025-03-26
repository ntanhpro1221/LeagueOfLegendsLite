using Unity.NetCode;

public struct InGameClientRpc : IRpcCommand {
    public BattleInitData initData;
}