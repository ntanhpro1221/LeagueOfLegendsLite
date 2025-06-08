using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public static class InitHelpers {
    public static void Bounty(
        ref DynamicBuffer<BountyBuffer>      bountyBuffer
      , ref EnabledRefRW<BountyBuffer>       bountyTrigger
      , ref BubleEnMap<BountyType, float_Q3> source) {
        // Enable
        bountyTrigger.ValueRW = true;

        // Set value
        for (int i = 0; i < EnumCount.Bounty; ++i)
            bountyBuffer[i] = source[(BountyType)i];
    }

    public static void StatsRaw(
        ref DynamicBuffer<StatsBuffer_Raw>  statsRaw
      , ref EnabledRefRW<StatsBuffer_Raw>   statsRawTrigger
      , ref BubleEnMap<StatsType, float_Q3> source) {
        // Enable
        statsRawTrigger.ValueRW = true;
        
        // Set value
        for (int i = 0; i < EnumCount.Stats; ++i)
            statsRaw[i] = source[(StatsType)i];
    }

    public static void StatsRaw(
        ref DynamicBuffer<StatsBuffer_Raw>         statsRaw
      , ref DynamicBuffer<StatsBuffer_RawPerLevel> statsRawPerLevel
      , ref EnabledRefRW<StatsBuffer_Raw>          statsRawTrigger
      , ref EnabledRefRW<StatsBuffer_RawPerLevel>  statsRawPerLevelTrigger
      , ref BubleEnMap<StatsType, float_Q3>        source
      , ref BubleEnMap<StatsType, float_Q3>        sourcePerLevel) {
        // Enable
        statsRawTrigger.ValueRW         = true;
        statsRawPerLevelTrigger.ValueRW = true;

        // Set value
        for (int i = 0; i < EnumCount.Stats; ++i) {
            statsRaw[i]         = source[(StatsType)i];
            statsRawPerLevel[i] = sourcePerLevel[(StatsType)i];
        }
    }
}