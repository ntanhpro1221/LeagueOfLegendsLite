using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct AssignFollowTagToMyChampionClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ChampionTag, GhostOwnerIsLocal>()
            .Build(ref state));
        state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp)
            .WithNone<CameraFollowTag>()
            .Build(ref state));
    }
        
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (
            localTrans
          , entity) in SystemAPI
            .Query<RefRO<LocalTransform>>()
            .WithAll<
                ChampionTag
              , GhostOwnerIsLocal>()
            .WithEntityAccess()) {
            ecb.AddComponent<CameraFollowTag>(entity);
            break; // just add to one target
        }

        ecb.Playback(state.EntityManager);
    }
}