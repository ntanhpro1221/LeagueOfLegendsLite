using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct AllChampionData : IComponentData {
    public BlobAssetReference<BubleEnMap<ChampionId, ChampionData, ChampionDataManaged>> _ChampionsRef;
    public BlobAssetReference<BubleEnMap<BountyId, float_Q3>>                          _CommonInitBountyRef;

    public ref BubleEnMap<ChampionId, ChampionData, ChampionDataManaged> Champions        => ref _ChampionsRef.Value;
    public ref BubleEnMap<BountyId, float_Q3>                          CommonInitBounty => ref _CommonInitBountyRef.Value;

    public void CreateBlobAssetReferenceInBaker(IBaker baker) {
        GameSO.Champ.CreateBlobAssetReferenceInBaker(out _ChampionsRef, baker, out _);
        GameSO.ChampCommonInitBounty.CreateBlobAssetReferenceInBaker(out _CommonInitBountyRef, baker, out _);
    }
}