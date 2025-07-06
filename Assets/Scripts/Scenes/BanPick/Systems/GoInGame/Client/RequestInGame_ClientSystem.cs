using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(InGameHandleSystemGroup))]
public partial struct RequestInGameClientSystem : ISystem {
    private EntityQuery pendingNetIdQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(pendingNetIdQuery = SystemAPI.QueryBuilder()
            .WithAll<NetworkId>()
            .WithNone<NetworkStreamInGame>()
            .Build());
    }

    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var entity = pendingNetIdQuery.GetSingletonEntity();

        // mark in game
        ecb.AddComponent<NetworkStreamInGame>(entity);

        // request in game to server
        ecb.SendRpc(new InGameClientRpc { playerName = BanPickBootstrapper.Instance.PlayerName });
    }
}