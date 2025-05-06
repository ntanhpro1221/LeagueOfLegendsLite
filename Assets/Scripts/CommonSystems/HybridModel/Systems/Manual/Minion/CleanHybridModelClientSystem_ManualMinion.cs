using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct CleanHybridModelClientSystem_ManualMinion : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                HybridModelCleanupData
              , ManualPoolingHybridModel_Cleanup>()
            .WithNone<LocalTransform>()
            .Build());
    }

    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (
            cleanupData
          , entity) in SystemAPI
            .Query<RefRO<HybridModelCleanupData>>()
            .WithAll<ManualPoolingHybridModel_Cleanup>()
            .WithNone<LocalTransform>()
            .WithEntityAccess()) {
            PoolCenter.Instance.Minion.Destroy(cleanupData.ValueRO.objectRef.Value);
            ecb.RemoveComponent<HybridModelCleanupData>(entity);
            ecb.RemoveComponent<ManualPoolingHybridModel_Cleanup>(entity);
        }
    }
}