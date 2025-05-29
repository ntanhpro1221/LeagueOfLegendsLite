using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(HandleConcreteSkillDataSystemGroup))]
public partial struct CleanupSkillDataSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<SkillData>()
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
            skillData
          , entity
            ) in SystemAPI
            .Query<
                RefRW<SkillData>
            >().WithNone<
                LocalTransform
            >().WithEntityAccess()) {
            skillData.ValueRW.Dispose();

            // Mark completed
            ecb.RemoveComponent<SkillData>(entity);
        }
    }
}