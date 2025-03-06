using System;
using BlobAssetExtend;
using PropertySetUtil;
using Unity.Entities;

public struct AllChampionData : IComponentData {
    public BlobHashMap<EquatableEnum<ChampionId>, ChampionData> champions;

    [Serializable]
    public class Managed : PropertySet<ChampionId, ChampionData.Managed> { }
}