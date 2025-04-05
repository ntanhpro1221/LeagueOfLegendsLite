using System;
using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using Unity.Entities;

[Serializable]
public struct ChampionData : IBlobBuildable<ChampionDataManaged>, IBlobBuildableSelf<ChampionData> {
    public BubleEnMap<StatsType, float_Q3> stats;
    public BubleEnMap<StatsType, float_Q3> statsPerLevel;

    public void BuildBlob(ref BlobBuilder builder, ChampionDataManaged source) {
        stats
            .BuildBlob(ref builder, source.stats);
        statsPerLevel
            .BuildBlob(ref builder, source.statsPerLevel);
    }

    public void BuildBlob(ref BlobBuilder builder, ref ChampionData source) {
        stats
            .BuildBlob(ref builder, ref source.stats);
        statsPerLevel
            .BuildBlob(ref builder, ref source.statsPerLevel);
    }
}