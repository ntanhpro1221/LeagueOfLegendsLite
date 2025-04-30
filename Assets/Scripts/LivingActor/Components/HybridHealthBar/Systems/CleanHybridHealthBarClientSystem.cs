using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct CleanHybridHealthBarClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<HybridHealthBarData>()
            .WithNone<LocalTransform>()
            .Build());
    }

    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (
                hybridData
              , entity)
            in SystemAPI
                .Query<RefRO<HybridHealthBarData>>()
                .WithNone<LocalTransform>()
                .WithEntityAccess()) {
            Object.Destroy(hybridData.ValueRO.transRef.Value.gameObject);
            ecb.RemoveComponent<HybridHealthBarData>(entity);
        }
    }
}