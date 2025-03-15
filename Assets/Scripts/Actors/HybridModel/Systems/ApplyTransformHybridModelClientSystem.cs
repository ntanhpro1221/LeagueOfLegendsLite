using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct ApplyTransformHybridModelClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        using EntityQueryBuilder queryBuilder = new(Allocator.Temp);
        state.RequireForUpdate(queryBuilder
            .WithAll<
                HybridModelData
              , LocalToWorld>()
            .Build(ref state));
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        foreach (var (
            hybridData
          , localToWorld) in SystemAPI.Query<
            RefRO<HybridModelData>
          , RefRO<LocalToWorld>>()) {
            var trans = hybridData.ValueRO.transformRef.Value;

            trans.position = localToWorld.ValueRO.Position;
            trans.rotation = localToWorld.ValueRO.Rotation;
        }

        ecb.Playback(state.EntityManager);
    }
}