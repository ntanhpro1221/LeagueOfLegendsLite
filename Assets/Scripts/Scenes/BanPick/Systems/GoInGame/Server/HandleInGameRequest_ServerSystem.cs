using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(InGameHandleSystemGroup))]
public partial struct HandleInGameRequest_ServerSystem : ISystem {
    [ReadOnly] private ComponentLookup<NetworkId> netIdLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<InGameClientRpc>();

        netIdLookup = SystemAPI.GetComponentLookup<NetworkId>(isReadOnly: true);

        state.EntityManager.CreateSingletonBuffer<IncomingClientBuffer>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        netIdLookup.Update(ref state);

        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var clientBuffer = SystemAPI.GetSingletonBuffer<IncomingClientBuffer>(isReadOnly: false);

        foreach (var (
            rpc
          , receiveRpc
            ) in SystemAPI
            .Query<
                RefRO<InGameClientRpc>
              , RpcHelpers.ReceiveRpcAspect
            >()) {
            var netId = receiveRpc.InGameProcess(ecb, netIdLookup);

            // add to member buffer
            clientBuffer.Add(new IncomingClientBuffer {
                playerName = rpc.ValueRO.playerName
              , netId      = netId
            });
        }
    }
}