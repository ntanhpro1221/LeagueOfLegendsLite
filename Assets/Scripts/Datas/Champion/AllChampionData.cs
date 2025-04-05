using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct AllChampionData : IComponentData {
    public BlobAssetReference<BubleEnMap<ChampionId, ChampionData, ChampionDataManaged>> _Ref;

    public ref BubleEnMap<ChampionId, ChampionData, ChampionDataManaged> Champions => ref _Ref.Value;
}