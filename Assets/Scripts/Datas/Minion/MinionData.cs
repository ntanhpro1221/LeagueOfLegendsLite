using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct MinionData : IBlobBuildable<MinionDataManaged>, IBlobBuildableSelf<MinionData> {
    public BubleEnMap<StatId, float_Q3>   stats;
    public BubleEnMap<BountyId, float_Q3> bounty;
    public int                            aggroRange;

    public void BuildBlob(ref BlobBuilder builder, MinionDataManaged source) {
        stats.BuildBlob(ref builder, source.stats);

        bounty.BuildBlob(ref builder, source.bounty);

        aggroRange = source.aggroRange;
    }

    public void BuildBlob(ref BlobBuilder builder, ref MinionData source) {
        stats.BuildBlob(ref builder, ref source.stats);

        bounty.BuildBlob(ref builder, ref source.bounty);

        aggroRange = source.aggroRange;
    }
}