using NGDtuanh.BubleAsset;
using Unity.Entities;

public static class InitHelpers {
    public static void Bounty(
        ref BountyData                       bounties
      , ref EnabledRefRW<BountyData>         bountyTrigger
      , ref BubleEnMap<BountyType, float_Q3> source) {
        // Enable
        bountyTrigger.ValueRW = true;

        // Set value
        ref var bountiesData = ref bounties.data;
        foreach (var index in Strum.Bounty.Info.Indexes)
            bountiesData[index] = source[index];
    }

    public static void StatsRaw(
        ref StatsData_Raw                   statsRaw
      , ref EnabledRefRW<StatsData_Raw>     statsRawTrigger
      , ref BubleEnMap<StatsType, float_Q3> source) {
        // Enable
        statsRawTrigger.ValueRW = true;

        // Set value
        ref var statsRawData = ref statsRaw.data;
        foreach (var index in Strum.Stats.Info.Indexes)
            statsRawData[index] = source[index];
    }

    public static void StatsRaw(
        ref StatsData_Raw                       statsRaw
      , ref StatsData_RawPerLevel               statsRawPerLevel
      , ref EnabledRefRW<StatsData_Raw>         statsRawTrigger
      , ref EnabledRefRW<StatsData_RawPerLevel> statsRawPerLevelTrigger
      , ref BubleEnMap<StatsType, float_Q3>     source
      , ref BubleEnMap<StatsType, float_Q3>     sourcePerLevel) {
        // Enable
        statsRawTrigger.ValueRW         = true;
        statsRawPerLevelTrigger.ValueRW = true;

        // Set value
        ref var statsRawData         = ref statsRaw.data;
        ref var statsRawPerLevelData = ref statsRawPerLevel.data;
        foreach (var index in Strum.Stats.Info.Indexes) {
            statsRawData[index]         = source[index];
            statsRawPerLevelData[index] = sourcePerLevel[index];
        }
    }
}