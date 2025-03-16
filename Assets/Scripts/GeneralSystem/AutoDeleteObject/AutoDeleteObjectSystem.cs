using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Just auto delete all object in <see cref="DeleteObjectData"/>
/// </summary>
public partial struct AutoDeleteObjectSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<DeleteObjectData>();
    }

    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (deleteObject, entity) in SystemAPI
            .Query<RefRO<DeleteObjectData>>()
            .WithEntityAccess()) {
            Object.Destroy(deleteObject.ValueRO.target);
            ecb.RemoveComponent<DeleteObjectData>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}