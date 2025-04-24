using Unity.NetCode;

public struct InGameClientRpc : IRpcCommand {
    public BattleInitData initData;

    public static implicit operator InGameClientRpc(BattleInitData initData) => new() { initData = initData };
}