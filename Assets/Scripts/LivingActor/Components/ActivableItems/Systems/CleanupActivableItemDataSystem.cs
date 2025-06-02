using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(HandleActivableItemDataSystemGroup))]
public partial struct CleanupActivableItemDataSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<AllActivableItemData>()
            .WithNone<LocalTransform>()
            .Build());
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        CleanupAll(ref state, ref ecb); 
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        CleanupAll(ref state, ref ecb); 
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    private void CleanupAll(ref SystemState state, ref EntityCommandBuffer ecb) {
        foreach (var (
            data
          , entity
            ) in SystemAPI
            .Query<
                RefRW<AllActivableItemData>
            >().WithNone<
                LocalTransform
            >().WithEntityAccess()) {
            data.ValueRW.Dispose();

            // Mark completed
            ecb.RemoveComponent<AllActivableItemData>(entity);
        }
    }
}