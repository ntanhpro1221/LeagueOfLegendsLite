using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(HandleInOut_Damage_Exp_Gold_SystemGroup))]
public partial struct Damage_Exp_Gold_Popup_ServerSystem : ISystem {
    private ComponentLookup<ChampConnection> connectionLookup;
    private ComponentLookup<GhostInstance>   ghostLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

        connectionLookup = SystemAPI.GetComponentLookup<ChampConnection>(
            isReadOnly: true);
        ghostLookup = SystemAPI.GetComponentLookup<GhostInstance>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();

        connectionLookup.Update(ref state);
        ghostLookup.Update(ref state);

        state.Dependency = new DamageJob {
            ecb              = ecb
          , connectionLookup = connectionLookup
          , ghostLookup      = ghostLookup
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new ExpJob {
            ecb              = ecb
          , connectionLookup = connectionLookup
          , ghostLookup      = ghostLookup
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new GoldJob {
            ecb              = ecb
          , connectionLookup = connectionLookup
          , ghostLookup      = ghostLookup
        }.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    private partial struct DamageJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;

        [ReadOnly] public ComponentLookup<ChampConnection> connectionLookup;
        [ReadOnly] public ComponentLookup<GhostInstance>   ghostLookup;

        [BurstCompile]
        private void Execute(
            in                   DynamicBuffer<IncomingDamageBuffer> source
          , in                   Entity                              entity
          , [EntityIndexInQuery] int                                 queryId) {
            var  receiverGhost   = new SpawnedGhost(ghostLookup[entity]);
            bool receiverIsChamp = connectionLookup.TryGetComponent(entity, out var receiverConnection);
            foreach (var item in source) {
                bool senderIsChamp = connectionLookup.TryGetComponent(item.source, out var senderConnection);

                if (!receiverIsChamp && !senderIsChamp) continue;

                var rpc = new DamagePopupRpc { damage = item.damage, receiver = receiverGhost };

                if (receiverIsChamp)
                    ecb.SendRpc(queryId, receiverConnection.entity, rpc);
                if (senderIsChamp && receiverConnection.entity != senderConnection.entity)
                    ecb.SendRpc(queryId, senderConnection.entity, rpc);
            }
        }
    }

    [BurstCompile]
    private partial struct ExpJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;

        [ReadOnly] public ComponentLookup<ChampConnection> connectionLookup;
        [ReadOnly] public ComponentLookup<GhostInstance>   ghostLookup;

        [BurstCompile]
        private void Execute(
            in                   DynamicBuffer<IncomingExpBuffer> source
          , in                   Entity                           entity
          , [EntityIndexInQuery] int                              queryId) {
            if (!connectionLookup.TryGetComponent(entity, out var receiverConnection)) return;

            var receiverGhost = new SpawnedGhost(ghostLookup[entity]);

            foreach (var item in source)
                ecb.SendRpc(queryId, receiverConnection.entity
                  , new ExpPopupRpc { exp = item.exp, receiver = receiverGhost });
        }
    }

    [BurstCompile]
    private partial struct GoldJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;

        [ReadOnly] public ComponentLookup<ChampConnection> connectionLookup;
        [ReadOnly] public ComponentLookup<GhostInstance>   ghostLookup;

        [BurstCompile]
        private void Execute(
            in                   DynamicBuffer<OutgoingGoldBuffer> source
          , in                   Entity                            entity
          , [EntityIndexInQuery] int                               queryId) {
            var senderGhost = new SpawnedGhost(ghostLookup[entity]);

            foreach (var item in source)
                if (connectionLookup.TryGetComponent(item.target, out var receiverConnection))
                    ecb.SendRpc(queryId, receiverConnection.entity
                      , new GoldPopupRpc { gold = item.gold, sender = senderGhost });
        }
    }
}