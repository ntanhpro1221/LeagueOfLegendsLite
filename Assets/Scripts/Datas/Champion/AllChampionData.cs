using System;
using NGDtuanh.BlobAssetExtend;
using Unity.Entities;

[Serializable]
public struct AllChampionData : IComponentData {
    public BlobAssetReference<BubleEnMap<ChampionId, ChampionData, ChampionDataManaged>> _Ref;

    public ref BubleEnMap<ChampionId, ChampionData, ChampionDataManaged> Champions => ref _Ref.Value;
}