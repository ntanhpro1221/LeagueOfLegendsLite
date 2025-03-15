using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct SpawnHybridModelClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        using EntityQueryBuilder queryBuilder = new(Allocator.Temp);
        state.RequireForUpdate(queryBuilder
            .WithAll<SpawnHybridModelRequest>()
            .Build(ref state));
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (
            spawnRequest
          , entity) in SystemAPI
            .Query<RefRO<SpawnHybridModelRequest>>()
            .WithEntityAccess()) {

            // spawn
            var model = Object.Instantiate(spawnRequest.ValueRO.prefabRef.Value);

            // set hybrid data
            var hybridData = new HybridModelData {
                transformRef = model.transform
              , animatorRef  = model.GetComponent<Animator>()
            };
            if (SystemAPI.HasComponent<HybridModelData>(entity))
                ecb.SetComponent(entity, hybridData);
            else ecb.AddComponent(entity, hybridData);

            // remove need spawn tag
            ecb.RemoveComponent<SpawnHybridModelRequest>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}