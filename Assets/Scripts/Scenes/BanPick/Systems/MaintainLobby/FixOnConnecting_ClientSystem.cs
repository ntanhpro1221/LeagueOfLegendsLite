using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// Fixing error when client connecting to server.<br/>
/// More info: https://discussions.unity.com/t/the-ghost-collection-contains-a-ghost-which-does-not-have-a-valid-prefab-on-the-client/939485/7
/// </summary>
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(GhostSimulationSystemGroup))]
[UpdateBefore(typeof(GhostCollectionSystem))]
public partial struct FixOnConnecting_ClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkStreamInGame>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var ghostPrefabs in SystemAPI.Query<DynamicBuffer<GhostCollectionPrefab>>())
            for (int i = 0; i < ghostPrefabs.Length; ++i)
                if (ghostPrefabs[i].GhostPrefab == Entity.Null)
                    ghostPrefabs.ElementAt(i).Loading = GhostCollectionPrefab.LoadingState.LoadingActive;
    }
}