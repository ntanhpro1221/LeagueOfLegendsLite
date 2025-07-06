using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// To add a new prefab collection, follow the following steps:<br/>
/// - Add to new buffer that inherit from <see cref="IPrefabBuffer"/> such as <see cref="ChampionPrefabBuffer"/>.<br/>
/// - Add data id to <see cref="PrefabIdData"/>.<br/>
/// - Add managed data table to <see cref="PrefabHubAuthoring"/> such as <see cref="PrefabHubAuthoring.championPrefabs"/>.<br/>
/// - Author prefab data by using <see cref="PrefabHubAuthoring.Baker.CreatePrefabData{TKey, TPrefabBuffer}"/>
/// </summary>
public class PrefabHubAuthoring : MonoBehaviour {
    [Header("-------------USELESS PREFABS--------------")]
    public List<GameObject> uselessPrefabs;

    [Header("     ------------PREFAB COLLECTIONS---------")]
    public CovEnumMap<ChampionId, GameObject> championPrefabs;

    public CovSerializedDictionary<TeamType, CovEnumMap<MinionId, GameObject>> minionPrefabs;
    public CovEnumMap<MonsterId, GameObject>                                   monsterPrefabs;
    public CovSerializedDictionary<TeamType, CovEnumMap<TowerId, GameObject>>  towerPrefabs;

    private class Baker : ExtendBaker<PrefabHubAuthoring> {
        public override void Bake(PrefabHubAuthoring authoring) {
            GetDynamicEntity(out var entity);

            // CREATE COMMON PREFAB REFERENCE
            foreach (var obj in authoring.uselessPrefabs) GetEntity(obj, TransformUsageFlags.Dynamic);

            // CREATE PREFAB COLLECTION DATA
            var prefabIdData = new PrefabIdData();
            CreatePrefabData<ChampionId, ChampionPrefabBuffer>(out prefabIdData._ChampionIdRef, authoring.championPrefabs, entity);
            CreatePrefabData<TeamType, MinionId, MinionPrefabBuffer>(out prefabIdData._MinionIdRef, authoring.minionPrefabs, entity);
            CreatePrefabData<MonsterId, MonsterPrefabBuffer>(out prefabIdData._MonsterIdRef, authoring.monsterPrefabs, entity);
            CreatePrefabData<TeamType, TowerId, TowerPrefabBuffer>(out prefabIdData._TowerIdRef, authoring.towerPrefabs, entity);
            AddComponent(entity, prefabIdData);
        }

        private void CreatePrefabData<TKey, TPrefabBuffer>(
            out BlobAssetReference<BlobMap<EqualEnum<TKey>, int>> idRef
          , ICovKVPCollection<TKey, GameObject>                   source
          , Entity                                                thisEntity)
            where TKey : unmanaged, Enum
            where TPrefabBuffer : unmanaged, IPrefabBuffer {
            // INIT PREFAB BUFFER
            var buffer = AddBuffer<TPrefabBuffer>(thisEntity);
            buffer.ResizeUninitialized(source.Count);

            // INIT BUILDER
            var     builder = new BlobBuilder(Allocator.Temp);
            ref var root    = ref builder.ConstructRoot<BlobMap<EqualEnum<TKey>, int>>();
            int     curId   = -1;

            // BUILD VALUE
            var idBuilder = builder.Allocate(ref root, source.Select(kvp => (EqualEnum<TKey>)kvp.Key).ToList());
            foreach (var (key, value) in source)
                buffer[idBuilder[key] = ++curId] = new TPrefabBuffer {
                    Entity = GetEntity(value, TransformUsageFlags.Dynamic)
                };

            // GEN REFERENCE AND ADD TO BAKER
            idRef = builder.CreateBlobAssetReference<BlobMap<EqualEnum<TKey>, int>>(Allocator.Persistent);
            AddBlobAsset(ref idRef, out _);
            builder.Dispose();
        }

        private void CreatePrefabData<TKey1, TKey2, TPrefabBuffer>(
            out BlobAssetReference<BlobMap<EqualEnum<TKey1>, BlobMap<EqualEnum<TKey2>, int>>> idRef
          , ICovKVPCollection<TKey1, ICovKVPCollection<TKey2, GameObject>>                    source
          , Entity                                                                            thisEntity)
            where TKey1 : unmanaged, Enum
            where TKey2 : unmanaged, Enum
            where TPrefabBuffer : unmanaged, IPrefabBuffer {
            // INIT PREFAB BUFFER
            var buffer = AddBuffer<TPrefabBuffer>(thisEntity);
            buffer.ResizeUninitialized(source.Sum(item => item.Value.Count));

            // INIT BUILDER
            var     builder = new BlobBuilder(Allocator.Temp);
            ref var root    = ref builder.ConstructRoot<BlobMap<EqualEnum<TKey1>, BlobMap<EqualEnum<TKey2>, int>>>();
            int     curId   = -1;

            // BUILD OUTER VALUE
            var outerIdBuilder = builder.Allocate(ref root
              , source.Select(kvp => (EqualEnum<TKey1>)kvp.Key).ToList());
            foreach (var (outerKey, outerValue) in source) {
                // BUILD INNER VALUE
                var innerIdBuilder = builder.Allocate(ref outerIdBuilder[outerKey]
                  , outerValue.Select(kvp => (EqualEnum<TKey2>)kvp.Key).ToList());
                foreach (var (innerKey, innerValue) in outerValue)
                    buffer[innerIdBuilder[innerKey] = ++curId] = new TPrefabBuffer {
                        Entity = GetEntity(innerValue, TransformUsageFlags.Dynamic)
                    };
            }

            // GEN REFERENCE AND ADD TO BAKER
            idRef = builder.CreateBlobAssetReference<BlobMap<EqualEnum<TKey1>, BlobMap<EqualEnum<TKey2>, int>>>(Allocator.Persistent);
            AddBlobAsset(ref idRef, out _);
            builder.Dispose();
        }
    }
}