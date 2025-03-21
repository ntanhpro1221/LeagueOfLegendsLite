using NGDtuanh.BlobAssetExtend;
using Unity.Entities;
using UnityEngine;

using TChampion = NGDtuanh.BlobAssetExtend.BubleEnMap
    <ChampionId, NGDtuanh.BlobAssetExtend.BubleEntity, UnityEngine.GameObject>;
using TChampionManaged = NGDtuanh.Collections.CovEnumMap
    <ChampionId, UnityEngine.GameObject>;
using TMonster = NGDtuanh.BlobAssetExtend.BubleEnMap
    <MonsterId, NGDtuanh.BlobAssetExtend.BubleEntity, UnityEngine.GameObject>;
using TMonsterManaged = NGDtuanh.Collections.CovEnumMap
    <MonsterId, UnityEngine.GameObject>;
using TTower = NGDtuanh.BlobAssetExtend.ShortCut.Buble_EnMap_EnMap
    <TeamType, TowerId, NGDtuanh.BlobAssetExtend.BubleEntity, UnityEngine.GameObject>;
using TTowerManaged = AYellowpaper.SerializedCollections.CovSerializedDictionary
    <TeamType, NGDtuanh.Collections.CovEnumMap<TowerId, UnityEngine.GameObject>>;
using TMinion = NGDtuanh.BlobAssetExtend.ShortCut.Buble_EnMap_EnMap
    <TeamType, MinionId, NGDtuanh.BlobAssetExtend.BubleEntity, UnityEngine.GameObject>;
using TMinionManaged = AYellowpaper.SerializedCollections.CovSerializedDictionary
    <TeamType, NGDtuanh.Collections.CovEnumMap<MinionId, UnityEngine.GameObject>>;

public struct PrefabHubData : IComponentData {
    public BlobAssetReference<TChampion> _ChampionsRef;
    public BlobAssetReference<TMonster>  _MonsterRef;
    public BlobAssetReference<TTower>    _TowerRef;
    public BlobAssetReference<TMinion>   _MinionRef;

    public ref TChampion Champions => ref _ChampionsRef.Value;
    public ref TMonster  Monster   => ref _MonsterRef.Value;
    public ref TTower    Tower     => ref _TowerRef.Value;
    public ref TMinion   Minion    => ref _MinionRef.Value;
}

public class PrefabHubAuthoring : MonoBehaviour {
    public TChampionManaged champions;
    public TMonsterManaged  monsters;
    public TTowerManaged    towers;
    public TMinionManaged   minions;

    private class Baker : Baker<PrefabHubAuthoring> {
        public override void Bake(PrefabHubAuthoring authoring) {
            var prefabHub = new PrefabHubData();

            authoring.champions.CreateBlobAssetReferenceInBaker(out prefabHub._ChampionsRef, this, out _);
            authoring.monsters.CreateBlobAssetReferenceInBaker(out prefabHub._MonsterRef, this, out _);
            authoring.towers.CreateBlobAssetReferenceInBaker(out prefabHub._TowerRef, this, out _);
            authoring.minions.CreateBlobAssetReferenceInBaker(out prefabHub._MinionRef, this, out _);

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, prefabHub);
        }
    }
}