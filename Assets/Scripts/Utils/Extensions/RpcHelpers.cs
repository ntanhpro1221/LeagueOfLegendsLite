using Unity.Entities;
using Unity.NetCode;

public static partial class RpcHelpers {
    public static Entity SendRpc<TData>(this EntityCommandBuffer ecb, bool isTag = false) where TData : unmanaged, IRpcCommand
        => SendRpc(ecb, default(TData), isTag);

    public static Entity SendRpc<TData>(this EntityManager em, bool isTag = false) where TData : unmanaged, IRpcCommand
        => SendRpc(em, default(TData), isTag);

    public static Entity SendRpc<TData>(this EntityCommandBuffer ecb, TData data, bool isTag = false) where TData : unmanaged, IRpcCommand {
        var entity = ecb.CreateEntity();
        ecb.AddComponent<SendRpcCommandRequest>(entity);
        if (!isTag) ecb.AddComponent(entity, data);
        return entity;
    }

    public static Entity SendRpc<TData>(this EntityManager em, TData data, bool isTag = false) where TData : unmanaged, IRpcCommand {
        var entity = em.CreateEntity();
        em.AddComponent<SendRpcCommandRequest>(entity);
        em.AddComponent<TData>(entity);
        if (!isTag) em.SetComponentData(entity, data);
        return entity;
    }

    public readonly partial struct ReceiveRpcAspect : IAspect {
        private readonly RefRO<ReceiveRpcCommandRequest> _ReceiveData;
        private readonly Entity                          _Entity;

        public ref readonly Entity SourceConnection => ref _ReceiveData.ValueRO.SourceConnection;

        public NetworkId CommonProcess(EntityCommandBuffer ecb, in ComponentLookup<NetworkId> netIdLookup) {
            // Destroy entity
            ecb.DestroyEntity(_Entity);

            return netIdLookup[SourceConnection];
        }

        public NetworkId InGameProcess(EntityCommandBuffer ecb, in ComponentLookup<NetworkId> netIdLookup) {
            // Destroy entity
            ecb.DestroyEntity(_Entity);

            // Mark InGame
            ecb.AddComponent<NetworkStreamInGame>(SourceConnection);

            return netIdLookup[SourceConnection];
        }
    }
}