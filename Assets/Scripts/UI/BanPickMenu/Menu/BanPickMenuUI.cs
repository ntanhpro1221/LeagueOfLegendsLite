using NGDtuanh.Singleton;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BanPickMenuUI : SceneSingleton<BanPickMenuUI> {
    [field: SerializeField] public ChampPickMenuUI ChampMenu    { get; private set; }
    [field: SerializeField] public StartGameBtn    StartGameBtn { get; private set; }

    [SerializeField] private MemberListUI _TeamBlue, _TeamRed;

    private bool _StartingGame;

    public void ForceUpdateTeamListUI(in DynamicBuffer<TeamMemberBuffer> buffer, in NetworkId myNetId) {
        _TeamBlue.ForceUpdateAllItemUI(buffer, myNetId);
        _TeamRed.ForceUpdateAllItemUI(buffer, myNetId);
    }

    public void OnQuit() {
        WorldHelpers.DestroyWorldsOfType(WorldFlags.Game);
        if (BanPickBootstrapper.Instance.IsHost) BanPickBootstrapper.Instance.StopBroadcastRoom();

        SceneManager.LoadScene(SceneNameHelper.HomeScene);
    }

    public void OnStartGame() {
        if (_StartingGame) return;
        _StartingGame = true;

        foreach (var server in World.All)
            if (server.Flags == WorldFlags.GameServer) {
                var em = server.EntityManager;

                em.SendRpc<StartGameServerRpc>();

                var battleSubSceneLoading = em.CreateEntity(typeof(BattleSubSceneLoading));
                em.SetComponentData(battleSubSceneLoading, new BattleSubSceneLoading {
                    Entity = SubSceneHub.Id.Battle.LoadAsyncTo(server.Unmanaged)
                });

                break;
            }
    }
}