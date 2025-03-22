using NGDtuanh.BlobAssetExtend;
using NGDtuanh.Collections;
using Unity.Entities;

/// <summary>
/// I just convert from enum to id.<br/>
/// You will use my id to get real value from correspond dynamic buffer.
/// </summary>
public struct PrefabIdData : IComponentData {
    public BlobAssetReference<BlobMap<EquatableEnum<ChampionId>, int>>                                 _ChampionIdRef;
    public BlobAssetReference<BlobMap<EquatableEnum<MonsterId>, int>>                                  _MonsterIdRef;
    public BlobAssetReference<BlobMap<EquatableEnum<TeamType>, BlobMap<EquatableEnum<TowerId>, int>>>  _TowerIdRef;
    public BlobAssetReference<BlobMap<EquatableEnum<TeamType>, BlobMap<EquatableEnum<MinionId>, int>>> _MinionIdRef;

    public ref BlobMap<EquatableEnum<ChampionId>, int>                                 ChampionId => ref _ChampionIdRef.Value;
    public ref BlobMap<EquatableEnum<MonsterId>, int>                                  MonsterId  => ref _MonsterIdRef.Value;
    public ref BlobMap<EquatableEnum<TeamType>, BlobMap<EquatableEnum<TowerId>, int>>  TowerId    => ref _TowerIdRef.Value;
    public ref BlobMap<EquatableEnum<TeamType>, BlobMap<EquatableEnum<MinionId>, int>> MinionId   => ref _MinionIdRef.Value;
}