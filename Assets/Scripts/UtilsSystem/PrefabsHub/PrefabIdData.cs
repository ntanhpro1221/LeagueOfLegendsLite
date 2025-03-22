using NGDtuanh.BlobAssetExtend;
using NGDtuanh.Collections;
using Unity.Entities;

/// <summary>
/// I just convert from enum to id.<br/>
/// You will use my id to get real value from correspond dynamic buffer.
/// </summary>
public struct PrefabIdData : IComponentData {
    public BlobAssetReference<BlobMap<EqualEnum<ChampionId>, int>>                                 _ChampionIdRef;
    public BlobAssetReference<BlobMap<EqualEnum<MonsterId>, int>>                                  _MonsterIdRef;
    public BlobAssetReference<BlobMap<EqualEnum<TeamType>, BlobMap<EqualEnum<TowerId>, int>>>  _TowerIdRef;
    public BlobAssetReference<BlobMap<EqualEnum<TeamType>, BlobMap<EqualEnum<MinionId>, int>>> _MinionIdRef;

    public ref BlobMap<EqualEnum<ChampionId>, int>                                 ChampionId => ref _ChampionIdRef.Value;
    public ref BlobMap<EqualEnum<MonsterId>, int>                                  MonsterId  => ref _MonsterIdRef.Value;
    public ref BlobMap<EqualEnum<TeamType>, BlobMap<EqualEnum<TowerId>, int>>  TowerId    => ref _TowerIdRef.Value;
    public ref BlobMap<EqualEnum<TeamType>, BlobMap<EqualEnum<MinionId>, int>> MinionId   => ref _MinionIdRef.Value;
}