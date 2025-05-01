using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct TowerData : IBlobBuildable<TowerDataManaged>, IBlobBuildableSelf<TowerData> {
    public BubleEnMap<StatsType, float_Q3> stats;
    public BubleEnMap<BountyType, int>     bounty;

    public void BuildBlob(ref BlobBuilder builder, TowerDataManaged source) {
        stats.BuildBlob(ref builder, source.stats);

        bounty.BuildBlob(ref builder, source.bounty);
    }

    public void BuildBlob(ref BlobBuilder builder, ref TowerData source) {
        stats.BuildBlob(ref builder, ref source.stats);

        bounty.BuildBlob(ref builder, ref source.bounty);
    }
}