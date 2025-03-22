using System;
using NGDtuanh.BlobAssetExtend;
using Unity.Entities;

[Serializable]
public struct ChampionData : IBlobBuildable<ChampionDataManaged> {
    public ChampionId                           id;
    public BubleEnMap<ChampionStatsType, float> stats;
    public BubleEnMap<ChampionStatsType, float> statsPerLevel;

    public void BuildBlob(ref BlobBuilder builder, ChampionDataManaged source) {
        id = source.id;

        stats.BuildBlob(ref builder, source.stats);

        statsPerLevel.BuildBlob(ref builder, source.statsPerLevel);
    }
}