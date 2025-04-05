using NGDtuanh.BubleAsset;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(BeforeInitAndUpdateStatsSystemGroup))]
public partial struct BuildRawStatsRefSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllChampionData>();
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        BuildChampion(ref state);
    }

    [BurstCompile]
    public void BuildChampion(ref SystemState state) {
        ref var champSource = ref SystemAPI.GetSingleton<AllChampionData>().Champions;

        foreach (var (
                champTag
              , rawStats
              , rawStatsPerLevel
              , rawStatsEnable
              , rawStatsPerLevelEnable)
            in SystemAPI
                .Query<
                    RefRO<ChampionTag>
                  , RefRW<RawStatsData>
                  , RefRW<RawStatsPerLevelData>
                  , EnabledRefRW<RawStatsData>
                  , EnabledRefRW<RawStatsPerLevelData>>()
                .WithAll<Simulate>()
                .WithDisabled<
                    RawStatsData
                  , RawStatsPerLevelData>()) {
            champSource[champTag.ValueRO.id].stats
                .CreateBlobAssetReference(out rawStats.ValueRW._Ref);
            champSource[champTag.ValueRO.id].statsPerLevel
                .CreateBlobAssetReference(out rawStatsPerLevel.ValueRW._Ref);

            rawStatsEnable.ValueRW         = true;
            rawStatsPerLevelEnable.ValueRW = true;
        }
    }
}