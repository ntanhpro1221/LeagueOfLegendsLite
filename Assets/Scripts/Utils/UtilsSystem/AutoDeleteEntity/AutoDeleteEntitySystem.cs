using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// Just auto delete all entity with tag <see cref="AutoDeleteTag"/>
/// </summary>
public partial struct AutoDeleteEntitySystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AutoDeleteTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (deleteTag, entity) in SystemAPI
            .Query<RefRO<AutoDeleteTag>>()
            .WithEntityAccess()) {
            if (deleteTag.ValueRO.WorldToDelete == WorldToDelete.Both ||
                state.WorldUnmanaged.IsClient() == (deleteTag.ValueRO.WorldToDelete == WorldToDelete.Client))
                ecb.DestroyEntity(entity);
            else ecb.RemoveComponent<AutoDeleteTag>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}