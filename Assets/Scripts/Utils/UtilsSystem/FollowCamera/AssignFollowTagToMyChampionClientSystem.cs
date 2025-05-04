using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct AssignFollowTagToMyChampionClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<ChampionTag, GhostOwnerIsLocal>()
            .WithNone<DummyTag>()
            .Build());
        state.RequireForUpdate(SystemAPI.QueryBuilder()
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
            tag
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