using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(InitBattleSystemGroup))]
public partial struct HandleInGameRequestServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PrefabIdData>();
        state.RequireForUpdate<ChampionPrefabBuffer>();
        state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp)
            .WithAll<
                InGameClientRpc
              , ReceiveRpcCommandRequest>()
            .Build(ref state));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var prefabId    = SystemAPI.GetSingleton<PrefabIdData>();
        var champPrefab = SystemAPI.GetSingletonBuffer<ChampionPrefabBuffer>(true);

        foreach (var (
            inGameClientRpc
          , receiveRpc
          , entity) in SystemAPI.Query<
                RefRO<InGameClientRpc>
              , RefRO<ReceiveRpcCommandRequest>>()
            .WithEntityAccess()) {
            // destroy request
            ecb.DestroyEntity(entity);

            // mark client in game
            ecb.AddComponent<NetworkStreamInGame>(receiveRpc.ValueRO.SourceConnection);

            // spawn player's champ
            var inGameData  = inGameClientRpc.ValueRO.initData;
            var champEntity = ecb.Instantiate(champPrefab[prefabId.ChampionId[inGameData.champion]].Entity);
            
            // set champ's team
            ecb.SetComponent(champEntity, new TeamTypeData {
                teamType = inGameData.teamType
            });

            // assign champ's owner to this client
            ecb.SetComponent(champEntity, new GhostOwner {
                NetworkId = SystemAPI.GetComponent<NetworkId>(receiveRpc.ValueRO.SourceConnection).Value
            });

            // link champ entity with this client connection
            ecb.AppendToBuffer(receiveRpc.ValueRO.SourceConnection, new LinkedEntityGroup {
                Value = champEntity
            });
        }

        ecb.Playback(state.EntityManager);
    }
}
