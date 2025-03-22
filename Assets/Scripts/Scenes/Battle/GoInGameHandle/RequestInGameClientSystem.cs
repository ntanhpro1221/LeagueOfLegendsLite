using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct RequestInGameClientSystem : ISystem {
    private EntityQuery _PendingNetworkIdQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        _PendingNetworkIdQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<NetworkId>()
            .WithNone<NetworkStreamInGame>()
            .Build(ref state);
        
        state.RequireForUpdate<BattleInitData>();
        state.RequireForUpdate(_PendingNetworkIdQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var battleInitData = SystemAPI.GetSingleton<BattleInitData>();
        
        foreach (var entity in _PendingNetworkIdQuery.ToEntityArray(Allocator.Temp)) {
            // mark in game
            ecb.AddComponent<NetworkStreamInGame>(entity);
        
            // request in game to server
            var rpcEntity = ecb.CreateEntity();
            ecb.AddComponent<SendRpcCommandRequest>(rpcEntity);
            ecb.AddComponent(rpcEntity, new InGameClientRpc {
                initData = battleInitData
            });
        }
        
        ecb.Playback(state.EntityManager);
    }
}