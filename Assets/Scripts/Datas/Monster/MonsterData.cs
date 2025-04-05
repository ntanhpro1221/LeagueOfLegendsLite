using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct MonsterData : IBlobBuildable<MonsterDataManaged>, IBlobBuildableSelf<MonsterData> {
    public BubleEnMap<StatsType, float_Q3> stats;
    public BubleEnMap<BountyType, int>     bounty;
    public int                             leashRange;

    public void BuildBlob(ref BlobBuilder builder, MonsterDataManaged source) {
        stats.BuildBlob(ref builder, source.stats);

        bounty.BuildBlob(ref builder, source.bounty);

        leashRange = source.leashRange;
    }

    public void BuildBlob(ref BlobBuilder builder, ref MonsterData source) {
        stats.BuildBlob(ref builder, source.stats);

        bounty.BuildBlob(ref builder, source.bounty);

        leashRange = source.leashRange;
    }
}