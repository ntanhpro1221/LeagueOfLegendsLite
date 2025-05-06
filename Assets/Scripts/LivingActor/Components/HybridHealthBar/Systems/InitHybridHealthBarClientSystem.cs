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
            requestData
          , requestTrigger
          , hybridData
          , hybridTrigger
          , entity) in SystemAPI
            .Query<
                RefRW<HybridHealthBarInitRequest>
              , EnabledRefRW<HybridHealthBarInitRequest>
              , RefRW<HybridHealthBarData>
              , EnabledRefRW<HybridHealthBarData>>()
            .WithPresent<HybridHealthBarData>()
            .WithEntityAccess()) {

            // spawn
            var healthBar = PoolCenter.Instance.HealthBar.Instantiate(requestData.ValueRO.healthBarType);

            // set root
            healthBar.transform.SetParent(canvasRoot);

            // Link healthBar with HybridHealthBarData
            hybridData.ValueRW = new HybridHealthBarData {
                deltaY   = requestData.ValueRO.deltaY
              , transRef = healthBar.transform as RectTransform
              , UIRef    = healthBar.GetComponent<HealthBarUI>()
            };
            
            // Add cleanup
            ecb.AddComponent(entity, new HybridHealthBarCleanup {
                healthBarRef = healthBar
            });

            // Mark request done
            requestTrigger.ValueRW = false;
            hybridTrigger.ValueRW  = true;
        }
    }
}