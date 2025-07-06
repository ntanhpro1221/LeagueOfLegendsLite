using Unity.Burst;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(StartGameHandleSystemGroup))]
public partial struct HandleStartGameServerRpcSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<StartGameServerRpc>();
    }

    public void OnUpdate(ref SystemState state) {
        var em = state.EntityManager;

        em.DestroyEntity(SystemAPI.QueryBuilder().WithAll<StartGameServerRpc>().Build());

        var battleSubSceneLoading = em.CreateEntity(typeof(BattleSubSceneLoading));
        em.SetComponentData(battleSubSceneLoading, new BattleSubSceneLoading {
            Entity = SubSceneHub.Id.Battle.LoadAsyncTo(state.WorldUnmanaged)
        });

        SceneManager.LoadSceneAsync(SceneNameHelper.BattleScene)!.completed += _ =>
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(BattleSceneLoaded));
    }
}