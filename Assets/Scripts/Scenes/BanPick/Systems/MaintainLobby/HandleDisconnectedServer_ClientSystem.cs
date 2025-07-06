using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine.SceneManagement;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateAfter(typeof(NetworkReceiveSystemGroup))]
public partial struct HandleDisconnectedServer_ClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkStreamDriver>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var events = SystemAPI.GetSingleton<NetworkStreamDriver>().ConnectionEventsForTick;
        if (events.Length == 0) return;

        foreach (var evt in events)
            if (evt.State == ConnectionState.State.Disconnected) {
                ReturnToHomeScene();

                break;
            }
    }

    [BurstDiscard]
    private void ReturnToHomeScene() {
        SceneManager.LoadSceneAsync(SceneNameHelper.HomeScene);
        WorldHelpers.DestroyWorldsOfType(WorldFlags.Game);
    }
}