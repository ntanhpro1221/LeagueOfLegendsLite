using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct InitHybridHealthBarClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<HybridHealthBarInitRequest>();
    }

    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var canvasRoot = MainCanvasRoot.Value;

        foreach (var (
            spawnRequest
          , entity) in SystemAPI
            .Query<RefRO<HybridHealthBarInitRequest>>()
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
            ecb.RemoveComponent<HybridHealthBarInitRequest>(entity);
        }
    }
}