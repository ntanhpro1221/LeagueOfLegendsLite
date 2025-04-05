using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct InitOwnChampionClientSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (
                localTrans
              , controlInputData
              , entity)
            in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                  , RefRW<PlayerInputData>>()
                .WithAll<
                    ChampionTag
                  , NeedInitTag
                  , GhostOwnerIsLocal>()
                .WithEntityAccess()) {
            // do something there in the future

            ecb.RemoveComponent<NeedInitTag>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}