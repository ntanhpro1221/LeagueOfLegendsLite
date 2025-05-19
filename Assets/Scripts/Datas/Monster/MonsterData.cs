using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct MonsterData : IBlobBuildable<MonsterDataManaged>, IBlobBuildableSelf<MonsterData> {
    public BubleEnMap<StatsType, float_Q3>  stats;
    public BubleEnMap<BountyType, float_Q3> bounty;
    public int                              leashRange;
    public uint                             respawnCDTick;

    public void BuildBlob(ref BlobBuilder builder, MonsterDataManaged source) {
        stats.BuildBlob(ref builder, source.stats);

        bounty.BuildBlob(ref builder, source.bounty);

        leashRange = source.leashRange;

        respawnCDTick = TickHelpers.CountTick(
            source.respawnCDTime
          , source.staticTickRate
          , TickHelpers.RoundMethod.Nearest);
    }

    public void BuildBlob(ref BlobBuilder builder, ref MonsterData source) {
        stats.BuildBlob(ref builder, ref source.stats);

        bounty.BuildBlob(ref builder, ref source.bounty);

        leashRange = source.leashRange;

        respawnCDTick = source.respawnCDTick;
    }
}