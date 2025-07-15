using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial struct CleanupEffectMapSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<EffectMap>()
            .WithNone<LocalTransform>()
            .Build());
    }

    private void DoCleanup(ref SystemState state, EntityCommandBuffer ecb) {
        foreach (var (
            effectMap
          , entity
            ) in SystemAPI
            .Query<
                RefRW<EffectMap>
            >().WithNone<
                LocalTransform
            >().WithEntityAccess()) {
            effectMap.ValueRW.Dispose();
            ecb.RemoveComponent<EffectMap>(entity);
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) => DoCleanup(ref state, SystemAPI
        .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
        .CreateCommandBuffer(state.WorldUnmanaged));

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);
        DoCleanup(ref state, ecb);
        ecb.Playback(state.EntityManager);
    }
}