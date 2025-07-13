using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Scenes;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(StartGameHandleSystemGroup))]
public partial struct WaitingBattleSceneReady_ClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BattleSceneLoaded>();
        state.RequireForUpdate<BattleSubSceneLoading>();
        state.RequireForUpdate<NetworkId>();
        state.RequireForUpdate<TeamMemberBuffer>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SceneSystem.IsSceneLoaded(
            state.WorldUnmanaged
          , SystemAPI.GetSingleton<BattleSubSceneLoading>().Entity))
            return;

        var em = state.EntityManager;

        // Send rpc
        em.DestroyEntity(SystemAPI.QueryBuilder().WithAny<
            BattleSceneLoaded
          , BattleSubSceneLoading
        >().Build());
        em.SendRpc<SpawnChampClientRpc>();

        // Create battle client data for client to access their info easier
        var clientDataEntity = em.CreateEntity();
        em.AddComponent<BattleClientData>(clientDataEntity);
        var member = SystemAPI.GetSingletonBuffer<TeamMemberBuffer>(isReadOnly: true);
        var netId  = SystemAPI.GetSingleton<NetworkId>();
        for (int i = 0; i < member.Length; ++i)
            if (member[i].netId.Value == netId.Value)
                em.SetComponentData(clientDataEntity, BattleClientData.BuildFrom(member[i]));
    }
}