using System;
using System.Collections.Generic;
using BlobAssetExtend;
using Unity.Entities;

[Serializable]
public struct ChampionData : IConstructableFromOtherVersion<ChampionDataManaged> {
    public ChampionId                                           id;
    public BlobHashMap<EquatableEnum<ChampionStatsType>, float> stats;
    public BlobHashMap<EquatableEnum<ChampionStatsType>, float> statsPerLevel;
    public Entity                                               prefab;

    public void Construct(BlobBuilder builder, IBaker baker, in ChampionDataManaged dataManaged) {
        id = dataManaged.id;

        Dictionary<EquatableEnum<ChampionStatsType>, float> statsConvert = new();
        foreach (var (key, value) in dataManaged.stats) statsConvert.Add(key, value);
        builder.SetHashMap(ref stats, statsConvert);
        
        Dictionary<EquatableEnum<ChampionStatsType>, float> statsPerLevelConvert = new();
        foreach (var (key, value) in dataManaged.statsPerLevel) statsPerLevelConvert.Add(key, value);
        builder.SetHashMap(ref statsPerLevel, statsPerLevelConvert);
        
        prefab = baker.GetEntity(dataManaged.prefab, TransformUsageFlags.Dynamic);
    }
}