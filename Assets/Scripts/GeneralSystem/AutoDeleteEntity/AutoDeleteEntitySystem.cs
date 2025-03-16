using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

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
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}