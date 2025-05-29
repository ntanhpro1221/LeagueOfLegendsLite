using NGDtuanh.BubleAsset;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(HandleConcreteSkillDataSystemGroup))]
public partial struct BuildSkillDataSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<AllChampionData>();
        state.RequireForUpdate<NeedBuildSkillData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        BuildForChampion(ref state, ref ecb);
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose(); 
    }

    [BurstCompile]
    private void BuildForChampion(ref SystemState state, ref EntityCommandBuffer ecb) {
        ref var champData = ref SystemAPI.GetSingleton<AllChampionData>().Champions;

        foreach (var (
            tag
          , entity
            ) in SystemAPI
            .Query<
                RefRO<ChampionTag>
            >().WithAll<
                NeedBuildSkillData
            >().WithNone<
                DummyTag
            >().WithEntityAccess()) {
            var data = new SkillData();

            data.CreateBlobAssetReference(ref champData[tag.ValueRO.id]);

            ecb.AddComponent(entity, data);

            // Mark build complete
            ecb.RemoveComponent<NeedBuildSkillData>(entity);
        }
    }
}