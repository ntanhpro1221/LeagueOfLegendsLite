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
                  , RefRW<MoveInputData>>()
                .WithAll<
                    ChampionTag
                  , NeedInitTag
                  , GhostOwnerIsLocal>()
                .WithEntityAccess()) {
            controlInputData.ValueRW.targetLocalPos = (float3_Q3)localTrans.ValueRO.Position;
            controlInputData.ValueRW.initialized    = true;

            ecb.RemoveComponent<NeedInitTag>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}