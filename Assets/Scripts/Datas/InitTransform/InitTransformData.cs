using Unity.Entities;

using TMonster = NGDtuanh.BubleAsset.ShortCut.Buble_EnMap_EnMap_Array
    <MonsterId, TeamType, InitTransform, UnityEngine.Transform>;
using TChampion = NGDtuanh.BubleAsset.ShortCut.Buble_EnMap_Array
    <TeamType, InitTransform, UnityEngine.Transform>;
using TMinion = NGDtuanh.BubleAsset.ShortCut.Buble_EnMap_EnMap_Array
    <LaneType, TeamType, InitTransform, UnityEngine.Transform>;
using TTower = NGDtuanh.BubleAsset.ShortCut.Buble_EnMap_EnMap_EnMap
    <TeamType, TowerId, LaneType, InitTransform, UnityEngine.Transform>;

public struct InitTransformData : IComponentData {
    public BlobAssetReference<TMonster>  _MonsterRef;
    public BlobAssetReference<TChampion> _ChampionRef;
    public BlobAssetReference<TMinion>   _MinionRef;
    public BlobAssetReference<TTower>    _TowerRef;

    public ref TMonster  Monster  => ref _MonsterRef.Value;
    public ref TChampion Champion => ref _ChampionRef.Value;
    public ref TMinion   Minion   => ref _MinionRef.Value;
    public ref TTower    Tower    => ref _TowerRef.Value;
}