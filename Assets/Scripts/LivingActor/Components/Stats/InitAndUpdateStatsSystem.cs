using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
public partial struct InitAndUpdateStatsSystem : ISystem {
    private BufferLookup<StatsData> statsLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        statsLookup = state.GetBufferLookup<StatsData>();

        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate<AllChampionData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        SetAllChampionStatsEnable(ref state);

        statsLookup.Update(ref state);

        ref var statsEnumId = ref SystemAPI.GetSingleton<EnumIndexData>().ChampionStatsType;
        ref var champData   = ref SystemAPI.GetSingleton<AllChampionData>().Champions;

        foreach (var (
                champTag
              , level
              , stats)
            in SystemAPI.Query<
                    RefRO<ChampionTag>
                  , RefRO<LevelData>
                  , DynamicBuffer<StatsData>>()
                .WithAll<Simulate>()) {
            ref var rawStats      = ref champData[champTag.ValueRO.id].stats;
            ref var statsPerLevel = ref champData[champTag.ValueRO.id].statsPerLevel;

            // just basic calculates base on level now
            for (int i = 0; i < stats.Length; ++i)
                stats.ElementAt(statsEnumId[rawStats.Value.Keys[i]]) = new StatsData {
                    FullValue = rawStats.Value.Values[i] + statsPerLevel.Value.Values[i] * (level.ValueRO.curLevel - 1)
                };
        }
    }

    [BurstCompile]
    public void SetAllChampionStatsEnable(ref SystemState state) {
        foreach (var (_, entity) in SystemAPI
            .Query<RefRO<ChampionTag>>()
            .WithDisabled<StatsData>()
            .WithAll<Simulate>()
            .WithEntityAccess())
            SystemAPI.SetBufferEnabled<StatsData>(entity, true);
    }
}