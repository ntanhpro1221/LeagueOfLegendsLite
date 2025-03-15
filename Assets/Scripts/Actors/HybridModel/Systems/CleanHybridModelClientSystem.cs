using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct CleanHybridModelClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        using EntityQueryBuilder queryBuilder = new(Allocator.Temp);
        state.RequireForUpdate(queryBuilder
            .WithAll<HybridModelData>()
            .WithNone<LocalTransform>()
            .Build(ref state));
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (
            hybridData
          , entity) in SystemAPI.Query<
                RefRO<HybridModelData>>()
            .WithEntityAccess()) {
            Object.Destroy(hybridData.ValueRO.transformRef.Value.gameObject);
            ecb.RemoveComponent<HybridModelData>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}