using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct CleanHybridHealthBarClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        using EntityQueryBuilder queryBuilder = new(Allocator.Temp);
        state.RequireForUpdate(queryBuilder
            .WithAll<HybridHealthBarData>()
            .WithNone<LocalTransform>()
            .Build(ref state));
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (
            hybridData
          , entity) in SystemAPI.Query<
                RefRO<HybridHealthBarData>>()
            .WithEntityAccess()) {
            Object.Destroy(hybridData.ValueRO.transRef.Value.gameObject);
            ecb.RemoveComponent<HybridHealthBarData>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}