using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct SpawnAndLinkHybridHealthBarClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<SpawnAndLinkHybridHealthBarRequest>()
            .Build());
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        var canvasRoot = MainCanvasRoot.Value;

        foreach (var (
            spawnRequest
          , entity) in SystemAPI
            .Query<RefRO<SpawnAndLinkHybridHealthBarRequest>>()
            .WithEntityAccess()) {

            // spawn
            var healthBar = Object.Instantiate(spawnRequest.ValueRO.healthBarPrefab.Value, canvasRoot);

            // Link healthBar with HybridHealthBarData
            var hybridData = new HybridHealthBarData {
                deltaY   = spawnRequest.ValueRO.deltaY
              , transRef = healthBar.transform as RectTransform
              , UIRef    = healthBar.GetComponent<HealthBarUI>()
            };
            if (SystemAPI.HasComponent<HybridHealthBarData>(entity))
                ecb.SetComponent(entity, hybridData);
            else ecb.AddComponent(entity, hybridData);

            // remove need spawn tag 
            ecb.RemoveComponent<SpawnAndLinkHybridHealthBarRequest>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}