using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct InitOwnChampionClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp)
            .WithAll<
                ChampionTag
              , Simulate
              , NeedInitTag
              , GhostOwnerIsLocal>()
            .Build(ref state));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (
                localTrans
              , entity)
            in SystemAPI
                .Query<
                    RefRO<LocalTransform>>()
                .WithAll<
                    Simulate
                  , NeedInitTag
                  , GhostOwnerIsLocal>()
                .WithEntityAccess()) {
            ecb.SetComponent(entity, new MoveInputData {
                targetLocalPos = (float3_Q3)localTrans.ValueRO.Position
            });

            ecb.RemoveComponent<NeedInitTag>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}