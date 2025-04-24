using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(InitBattleSystemGroup))]
public partial struct HandleInGameRequestServerSystem : ISystem {
    [ReadOnly] public ComponentLookup<NetworkId> netIdLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PrefabIdData>();
        state.RequireForUpdate<ChampionPrefabBuffer>();
        state.RequireForUpdate<InGameClientRpc>();

        netIdLookup = SystemAPI.GetComponentLookup<NetworkId>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        netIdLookup.Update(ref state);

        state.Dependency = new Job {
            ecbParallel = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter()
          , prefabId = SystemAPI.GetSingleton<PrefabIdData>()
          , champPrefab = SystemAPI.GetSingletonBuffer<ChampionPrefabBuffer>(
                isReadOnly: true)
          , netIdLookup = netIdLookup
        }.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    private partial struct Job : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecbParallel;
        public PrefabIdData                       prefabId;

        [ReadOnly] public DynamicBuffer<ChampionPrefabBuffer> champPrefab;
        [ReadOnly] public ComponentLookup<NetworkId>          netIdLookup;

        public void Execute(
            in                   InGameClientRpc          inGameClientRpc
          , in                   ReceiveRpcCommandRequest receiveRpc
          , in                   Entity                   entity
          , [EntityIndexInQuery] int                      queryId) {
            // destroy request
            ecbParallel.DestroyEntity(queryId, entity);

            // mark client in game
            ecbParallel.AddComponent<NetworkStreamInGame>(queryId, receiveRpc.SourceConnection);

            // spawn player's champ
            var inGameData  = inGameClientRpc.initData;
            var champEntity = ecbParallel.Instantiate(queryId, champPrefab[prefabId.ChampionId[inGameData.champion]].Entity);

            // set champ's team
            ecbParallel.SetComponent<TeamTypeData>(queryId, champEntity, inGameData.teamType);

            // assign champ's owner to this client
            ecbParallel.SetComponent(queryId, champEntity, new GhostOwner {
                NetworkId = netIdLookup[receiveRpc.SourceConnection].Value
            });

            // link champ entity with this client connection
            ecbParallel.AppendToBuffer<LinkedEntityGroup>(queryId, receiveRpc.SourceConnection, champEntity);
        }
    }
}