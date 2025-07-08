using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct AssignFollowTagToMyChampionClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            // We do not need enable-able here so this is fine!
            .WithAll<ChampionTag, GhostOwnerIsLocal>()
            .WithNone<DummyTag>()
            .Build());
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            // We do not need enable-able here so this is fine!
            .WithNone<CameraFollowTag>()
            .Build());
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (
            _
          , entity) in SystemAPI
            .Query<ChampionTag>()
            .WithAll<GhostOwnerIsLocal>()
            .WithNone<DummyTag>()
            .WithEntityAccess()) {
            ecb.AddComponent<CameraFollowTag>(entity);
            break; // just add to one target
        }
    }
}