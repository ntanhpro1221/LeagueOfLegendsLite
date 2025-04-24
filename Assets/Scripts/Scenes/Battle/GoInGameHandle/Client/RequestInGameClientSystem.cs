using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(InitBattleSystemGroup))]
public partial struct RequestInGameClientSystem : ISystem {
    private EntityQuery _PendingNetworkIdQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<BattleInitData>();
        state.RequireForUpdate(_PendingNetworkIdQuery = SystemAPI.QueryBuilder()
            .WithAll<NetworkId>()
            .WithNone<NetworkStreamInGame>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var battleInitData = SystemAPI.GetSingleton<BattleInitData>();

        using var pendingNetworkIds = _PendingNetworkIdQuery.ToEntityArray(Allocator.Temp);

        foreach (var entity in pendingNetworkIds) {
            // mark in game
            ecb.AddComponent<NetworkStreamInGame>(entity);

            // request in game to server
            var rpcEntity = ecb.CreateEntity();
            ecb.AddComponent<SendRpcCommandRequest>(rpcEntity);
            ecb.AddComponent<InGameClientRpc>(rpcEntity, battleInitData);
        }
    }
}