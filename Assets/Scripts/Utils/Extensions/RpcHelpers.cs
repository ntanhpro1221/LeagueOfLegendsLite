using Unity.Entities;
using Unity.NetCode;

public static partial class RpcHelpers {
    public static Entity SendRpc<TData>(this EntityCommandBuffer ecb, TData data = default)
        where TData : unmanaged, IRpcCommand {
        var entity = ecb.CreateEntity();
        ecb.AddComponent<SendRpcCommandRequest>(entity);
        ecb.AddComponent(entity, data);
        return entity;
    }

    public static Entity SendRpc<TData>(this EntityCommandBuffer.ParallelWriter ecb, int queryId, TData data = default)
        where TData : unmanaged, IRpcCommand {
        var entity = ecb.CreateEntity(queryId);
        ecb.AddComponent<SendRpcCommandRequest>(queryId, entity);
        ecb.AddComponent(queryId, entity, data);
        return entity;
    }

    public static Entity SendRpc<TData>(this EntityManager em, TData data = default)
        where TData : unmanaged, IRpcCommand {
        var entity = em.CreateEntity();
        em.AddComponent<SendRpcCommandRequest>(entity);
        em.AddComponentData(entity, data);
        return entity;
    }

    public static Entity SendRpc<TData>(this EntityCommandBuffer ecb, Entity target, TData data = default)
        where TData : unmanaged, IRpcCommand {
        var entity = ecb.CreateEntity();
        ecb.AddComponent(entity, new SendRpcCommandRequest { TargetConnection = target });
        ecb.AddComponent(entity, data);
        return entity;
    }

    public static Entity SendRpc<TData>(this EntityCommandBuffer.ParallelWriter ecb, int queryId, Entity target, TData data = default)
        where TData : unmanaged, IRpcCommand {
        var entity = ecb.CreateEntity(queryId);
        ecb.AddComponent(queryId, entity, new SendRpcCommandRequest { TargetConnection = target });
        ecb.AddComponent(queryId, entity, data);
        return entity;
    }

    public static Entity SendRpc<TData>(this EntityManager em, Entity target, TData data = default)
        where TData : unmanaged, IRpcCommand {
        var entity = em.CreateEntity();
        em.AddComponentData(entity, new SendRpcCommandRequest { TargetConnection = target });
        em.AddComponentData(entity, data);
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