using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(HandleActivableItemDataSystemGroup))]
public partial struct BuildActivableItemDataSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllChampionData>();
        state.RequireForUpdate<NeedBuildActivableItemData>();
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
                NeedBuildActivableItemData
            >().WithNone<
                DummyTag
            >().WithEntityAccess()) {
            var data = new AllActivableItemData();

            // Set all skill
            data.Init(ref champData[tag.ValueRO.id]);

            ecb.AddComponent(entity, data);

            // Mark build complete
            ecb.RemoveComponent<NeedBuildActivableItemData>(entity);
        }
    }
}