using NGDtuanh.BubleAsset;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(BeforeInitAndUpdateStatsSystemGroup))]
public partial struct BuildRawStatsRefSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();

        state.RequireForUpdate<AllChampionData>();
        state.RequireForUpdate<AllMinionData>();
        state.RequireForUpdate<AllTowerData>();
        state.RequireForUpdate<AllMonsterData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        BuildChampion(ref state, ecb);
        BuildMinion(ref state, ecb);
        BuildTower(ref state, ecb);
        BuildMonster(ref state, ecb);

        ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    private void BuildChampion(ref SystemState state, in EntityCommandBuffer ecb) {
        ref var champSource = ref SystemAPI.GetSingleton<AllChampionData>().Champions;

        foreach (var (champTag, entity)
            in SystemAPI
                .Query<RefRO<ChampionTag>>()
                .WithAll<
                    NeedBuildRawStats
                  , Simulate>()
                .WithEntityAccess()) {
            var rawStats         = new RawStatsData();
            var rawStatsPerLevel = new RawStatsPerLevelData();

            champSource[champTag.ValueRO.id].stats
                .CreateBlobAssetReference(out rawStats._Ref);
            champSource[champTag.ValueRO.id].statsPerLevel
                .CreateBlobAssetReference(out rawStatsPerLevel._Ref);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            ecb.AddComponent(entity, rawStats);
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            ecb.AddComponent(entity, rawStatsPerLevel);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            ecb.RemoveComponent<NeedBuildRawStats>(entity);
        }
    }

    [BurstCompile]
    private void BuildMinion(ref SystemState state, in EntityCommandBuffer ecb) {
        ref var minionSource = ref SystemAPI.GetSingleton<AllMinionData>().Minions;

        foreach (var (minionTag, entity)
            in SystemAPI
                .Query<RefRO<MinionTag>>()
                .WithAll<
                    NeedBuildRawStats
                  , Simulate>()
                .WithEntityAccess()) {
            var rawStats = new RawStatsData();

            minionSource[minionTag.ValueRO.id].stats
                .CreateBlobAssetReference(out rawStats._Ref);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            ecb.AddComponent(entity, rawStats);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            ecb.RemoveComponent<NeedBuildRawStats>(entity);
        }
    }

    [BurstCompile]
    private void BuildTower(ref SystemState state, in EntityCommandBuffer ecb) {
        ref var towerSource = ref SystemAPI.GetSingleton<AllTowerData>().Towers;

        foreach (var (towerTag, entity)
            in SystemAPI
                .Query<RefRO<TowerTag>>()
                .WithAll<
                    NeedBuildRawStats
                  , Simulate>()
                .WithEntityAccess()) {
            var rawStats = new RawStatsData();

            towerSource[towerTag.ValueRO.id].stats
                .CreateBlobAssetReference(out rawStats._Ref);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            ecb.AddComponent(entity, rawStats);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            ecb.RemoveComponent<NeedBuildRawStats>(entity);
        }
    }

    [BurstCompile]
    private void BuildMonster(ref SystemState state, in EntityCommandBuffer ecb) {
        ref var monsterSource = ref SystemAPI.GetSingleton<AllMonsterData>().Monsters;

        foreach (var (monsterTag, entity)
            in SystemAPI
                .Query<RefRO<MonsterTag>>()
                .WithAll<
                    NeedBuildRawStats
                  , Simulate>()
                .WithEntityAccess()) {
            var rawStats = new RawStatsData();

            monsterSource[monsterTag.ValueRO.id].stats
                .CreateBlobAssetReference(out rawStats._Ref);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            ecb.AddComponent(entity, rawStats);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            ecb.RemoveComponent<NeedBuildRawStats>(entity);
        }
    }
}