using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Scenes;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct SpawnChamp_ServerSystem : ISystem {
    private const int MAX_INIT_SLOT = 5;

    [ReadOnly] private ComponentLookup<NetworkId> netIdLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<BattleSubSceneLoading>();
        state.RequireForUpdate<SpawnChampClientRpc>();
        state.RequireForUpdate<TeamMemberBuffer>();
        state.RequireForUpdate<ChampionPrefabBuffer>();
        state.RequireForUpdate<PrefabIdData>();

        netIdLookup = SystemAPI.GetComponentLookup<NetworkId>(isReadOnly: true);
    }

    public void OnUpdate(ref SystemState state) {
        if (!SceneSystem.IsSceneLoaded(
            state.WorldUnmanaged
          , SystemAPI.GetSingleton<BattleSubSceneLoading>().Entity))
            return;

        netIdLookup.Update(ref state);

        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var memberBuffer = SystemAPI.GetSingletonBuffer<TeamMemberBuffer>(isReadOnly: true);
        var champPrefab  = SystemAPI.GetSingletonBuffer<ChampionPrefabBuffer>(isReadOnly: true);
        var prefabId     = SystemAPI.GetSingleton<PrefabIdData>();

        foreach (var (
            _
          , receiveRpc
            ) in SystemAPI
            .Query<
                RefRO<SpawnChampClientRpc>
              , RpcHelpers.ReceiveRpcAspect
            >()) {
            var netId = receiveRpc.CommonProcess(ecb, netIdLookup);

            TeamMemberBuffer member = default;
            foreach (var item in memberBuffer)
                if (item.netId.Value == netId.Value) {
                    member = item;
                    break;
                }

            ChampionOrderInTeam orderData = default;
            foreach (var item in memberBuffer)
                if (item.netId.Value == netId.Value) break;
                else if (item.team   == member.team) orderData.order = (orderData.order + 1) % MAX_INIT_SLOT;

            // spawn player's champ
            var champEntity = ecb.Instantiate(champPrefab[prefabId.ChampionId[member.champ]].Entity);

            // set champ's order in team
            ecb.SetComponent(champEntity, orderData);

            // set champ's team
            ecb.SetComponent<TeamTypeData>(champEntity, member.team);

            // assign champ's owner to this client
            ecb.SetComponent(champEntity, new GhostOwner { NetworkId = netId.Value });

            // link champ entity with this client connection
            ecb.AppendToBuffer<LinkedEntityGroup>(receiveRpc.SourceConnection, champEntity);
        }
    }
}