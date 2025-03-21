using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct SpawnAndLinkHybridModelClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        using EntityQueryBuilder queryBuilder = new(Allocator.Temp);
        state.RequireForUpdate(queryBuilder
            .WithAll<SpawnAndLinkHybridModelRequest>()
            .Build(ref state));
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (
            spawnRequest
          , entity) in SystemAPI
            .Query<RefRO<SpawnAndLinkHybridModelRequest>>()
            .WithEntityAccess()) {

            // spawn
            var model = Object.Instantiate(spawnRequest.ValueRO.prefabRef.Value);

            // Link model with HybridModelData
            var hybridData = new HybridModelData {
                transformRef = model.transform
              , animatorRef  = model.GetComponent<Animator>()
            };
            if (SystemAPI.HasComponent<HybridModelData>(entity))
                ecb.SetComponent(entity, hybridData);
            else ecb.AddComponent(entity, hybridData);

            // remove need spawn tag
            ecb.RemoveComponent<SpawnAndLinkHybridModelRequest>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}