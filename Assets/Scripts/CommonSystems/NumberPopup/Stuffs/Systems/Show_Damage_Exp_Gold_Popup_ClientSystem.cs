using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct Show_Damage_Exp_Gold_Popup_ClientSystem : ISystem {
    [ReadOnly] private ComponentLookup<NetworkId>       netLookup;
    [ReadOnly] private ComponentLookup<HybridModelData> modelLookup;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SpawnedGhostEntityMap>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireAnyForUpdate(
            SystemAPI.QueryBuilder().WithAll<DamagePopupRpc>().Build()
          , SystemAPI.QueryBuilder().WithAll<ExpPopupRpc>().Build()
          , SystemAPI.QueryBuilder().WithAll<GoldPopupRpc>().Build());

        netLookup = SystemAPI.GetComponentLookup<NetworkId>(
            isReadOnly: true);
        modelLookup = SystemAPI.GetComponentLookup<HybridModelData>(
            isReadOnly: true);
    }

    public void OnUpdate(ref SystemState state) {
        netLookup.Update(ref state);
        modelLookup.Update(ref state);

        var ghostMap = SystemAPI.GetSingleton<SpawnedGhostEntityMap>().Value;
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (rpc, receiveRpc) in SystemAPI.Query<RefRO<DamagePopupRpc>, RpcHelpers.ReceiveRpcAspect>()) {
            receiveRpc.CommonProcess(ecb, netLookup);
            NumberPopup.Id.NomDmg.Popup((int)rpc.ValueRO.damage
              , modelLookup[ghostMap[rpc.ValueRO.receiver]].transformRef);
        }

        foreach (var (rpc, receiveRpc) in SystemAPI.Query<RefRO<ExpPopupRpc>, RpcHelpers.ReceiveRpcAspect>()) {
            receiveRpc.CommonProcess(ecb, netLookup);
            NumberPopup.Id.Exp.Popup(rpc.ValueRO.exp
              , modelLookup[ghostMap[rpc.ValueRO.receiver]].transformRef);
        }

        foreach (var (rpc, receiveRpc) in SystemAPI.Query<RefRO<GoldPopupRpc>, RpcHelpers.ReceiveRpcAspect>()) {
            receiveRpc.CommonProcess(ecb, netLookup);
            NumberPopup.Id.Gold.Popup((int)rpc.ValueRO.gold
              , modelLookup[ghostMap[rpc.ValueRO.sender]].transformRef);
        }
    }
}