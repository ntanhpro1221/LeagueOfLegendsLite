using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct InitOwnChampionClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp)
            .WithAll<
                ChampionTag
              , Simulate
              , ClientNeedInitTag
              , GhostOwnerIsLocal>()
            .Build(ref state));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (
                localTrans
              , _
              , entity)
            in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                  , RefRO<ChampionTag>>()
                .WithAll<
                    Simulate
                  , ClientNeedInitTag
                  , GhostOwnerIsLocal>()
                .WithEntityAccess()) {
            ecb.SetComponent(entity, new MoveInputData {
                targetPos = localTrans.ValueRO.Position
            });

            ecb.RemoveComponent<ClientNeedInitTag>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}