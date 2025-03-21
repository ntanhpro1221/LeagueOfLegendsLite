using System;
using NGDtuanh.BlobAssetExtend;
using Unity.Entities;

[Serializable]
public struct ChampionData : IBlobBuildable<ChampionDataManaged> {
    public ChampionId                           id;
    public BubleEnMap<ChampionStatsType, float> stats;
    public BubleEnMap<ChampionStatsType, float> statsPerLevel;
    public Entity                               prefab;

    public void BuildBlob(ref BlobBuilder builder, ChampionDataManaged source, IBaker baker) {
        id = source.id;

        stats.BuildBlob(ref builder, source.stats, baker);

        statsPerLevel.BuildBlob(ref builder, source.statsPerLevel, baker);

        prefab = baker.GetEntity(source.prefab, TransformUsageFlags.Dynamic);
    }
}