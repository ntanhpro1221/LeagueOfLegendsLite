using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NGDtuanh.BlobAssetExtend;
using NGDtuanh.BlobAssetExtend.ShortCut;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

public class InitTransformDataAuthoring : MonoBehaviour {
    public CovEnumMap<MonsterId
      , CovSerializedDictionary<TeamType
          , List<Transform>>> monster;

    public CovSerializedDictionary<TeamType
      , List<Transform>> champion;

    public CovSerializedDictionary<LaneType
      , CovSerializedDictionary<TeamType
          , List<Transform>>> minion;

    public CovSerializedDictionary<TeamType
      , CovEnumMap<TowerId
          , CovSerializedDictionary<LaneType, Transform>>> tower;

    private class Baker : Baker<InitTransformDataAuthoring> {
        public override void Bake(InitTransformDataAuthoring authoring) {
            var dataRef = new InitTransformData();

            authoring.monster.CreateBlobAssetReferenceInBaker(out dataRef._MonsterRef, this, out _);
            authoring.champion.CreateBlobAssetReferenceInBaker(out dataRef._ChampionRef, this, out _);
            authoring.minion.CreateBlobAssetReferenceInBaker(out dataRef._MinionRef, this, out _);
            authoring.tower.CreateBlobAssetReferenceInBaker(out dataRef._TowerRef, this, out _);

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, dataRef);
        }
    }
}