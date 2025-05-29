using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct ChampionData : IBlobBuildable<ChampionDataManaged>, IBlobBuildableSelf<ChampionData> {
    public BubleEnMap<StatsType, float_Q3>                     stats;
    public BubleEnMap<StatsType, float_Q3>                     statsPerLevel;
    public ActivableItemData                                   passive;
    public BubleArray<ActivableItemData, IActivableItemDataSO> skills;

    public void BuildBlob(ref BlobBuilder builder, ChampionDataManaged source) {
        stats
            .BuildBlob(ref builder, source.stats);
        statsPerLevel
            .BuildBlob(ref builder, source.statsPerLevel);
        passive
            .BuildBlob(ref builder, source.passive);
        skills
            .BuildBlob(ref builder, source.skills);
    }

    public void BuildBlob(ref BlobBuilder builder, ref ChampionData source) {
        stats
            .BuildBlob(ref builder, ref source.stats);
        statsPerLevel
            .BuildBlob(ref builder, ref source.statsPerLevel);
        passive
            .BuildBlob(ref builder, ref source.passive);
        skills
            .BuildBlob(ref builder, ref source.skills);
    }
}