using System;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


[UpdateInGroup(typeof(InitAndUpdateStatsSystemGroup))]
public partial struct InitAndUpdateStatsSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<EnumIndexData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        ref var statsId  = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;
        ref var statsKey = ref statsId.Value.Keys;

        // Enable all stats
        foreach (var statsEnable in SystemAPI
            .Query<EnabledRefRW<StatsBuffer>>()
            .WithDisabled<StatsBuffer>()
            .WithAll<
                RawStatsData
              , Simulate>())
            statsEnable.ValueRW = true;

        // Get raw value
        foreach (var (
                stats
              , rawStats)
            in SystemAPI.Query<
                    DynamicBuffer<StatsBuffer>
                  , RefRO<RawStatsData>>()
                .WithAll<Simulate>())
            for (int i = 0; i < statsKey.Length; ++i)
                stats.ElementAt(statsId[statsKey[i]]).value
                    = rawStats.ValueRO[statsKey[i]];

        // Apply value (if level exists)
        foreach (var (
                stats
              , rawStatsPerLevel
              , levelData)
            in SystemAPI.Query<
                    DynamicBuffer<StatsBuffer>
                  , RefRO<RawStatsPerLevelData>
                  , RefRO<LevelData>>()
                .WithAll<Simulate>())
            for (int i = 0; i < statsKey.Length; ++i)
                stats.ElementAt(statsId[statsKey[i]]).value
                    += rawStatsPerLevel.ValueRO[statsKey[i]] * (levelData.ValueRO.curLevel - 1);

        // Apply buff
        foreach (var (
                stats
              , buffs)
            in SystemAPI.Query<
                    DynamicBuffer<StatsBuffer>
                  , DynamicBuffer<BuffBuffer>>()
                .WithAll<Simulate>())
            for (int i = 0; i < statsKey.Length; ++i)
                stats.ElementAt(i).value
                    = (stats[i].value + buffs[i].add) * buffs[i].mul;
    }
}