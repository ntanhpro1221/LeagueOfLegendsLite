using System;
using BlobAssetExtend;
using Unity.Entities;

[Serializable]
public struct AllChampionData : IComponentData {
    public BlobAssetReference<BlobHashMap<EquatableEnum<ChampionId>, ChampionData>> champions;
}