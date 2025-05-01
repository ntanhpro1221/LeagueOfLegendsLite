using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct MinionWaveData :
    IBlobBuildable<MinionWaveDataManaged>
  , IBlobBuildableSelf<MinionWaveData> {
    public BubleArray<MinionId> minions;
    public bool                 isFixedSpawn;
    public float                firstWaveTime;
    public float                waveInterval;

    public void BuildBlob(ref BlobBuilder builder, MinionWaveDataManaged source) {
        minions.BuildBlob(ref builder, source.minions);
        isFixedSpawn  = source.isFixedSpawn;
        firstWaveTime = source.firstWaveTime;
        waveInterval  = source.waveInterval;
    }

    public void BuildBlob(ref BlobBuilder builder, ref MinionWaveData source) {
        minions.BuildBlob(ref builder, ref source.minions);
        isFixedSpawn  = source.isFixedSpawn;
        firstWaveTime = source.firstWaveTime;
        waveInterval  = source.waveInterval;
    }
}