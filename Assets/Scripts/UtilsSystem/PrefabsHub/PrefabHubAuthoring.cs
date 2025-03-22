using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NGDtuanh.BlobAssetExtend;
using NGDtuanh.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// To add a new element, follow the following steps:<br/>
/// - Add to new buffer that inherit from <see cref="IPrefabBuffer"/> such as <see cref="ChampionPrefabBuffer"/>.<br/>
/// - Add data id to <see cref="PrefabIdData"/>.<br/>
/// - Add managed data table to <see cref="PrefabHubAuthoring"/> such as <see cref="PrefabHubAuthoring.championPrefabs"/>.<br/>
/// - Author prefab data by using <see cref="PrefabHubAuthoring.Baker.CreatePrefabData{TKey, TPrefabBuffer}"/>
/// </summary>
public class PrefabHubAuthoring : MonoBehaviour {
    public CovEnumMap<ChampionId, GameObject>                                  championPrefabs;
    public CovEnumMap<MonsterId, GameObject>                                   monsterPrefabs;
    public CovSerializedDictionary<TeamType, CovEnumMap<TowerId, GameObject>>  towerPrefabs;
    public CovSerializedDictionary<TeamType, CovEnumMap<MinionId, GameObject>> minionPrefabs;

    private class Baker : Baker<PrefabHubAuthoring> {
        public override void Bake(PrefabHubAuthoring authoring) {
            var thisEntity   = GetEntity(TransformUsageFlags.Dynamic);
            var prefabIdData = new PrefabIdData();

            CreatePrefabData<ChampionId, ChampionPrefabBuffer>(out prefabIdData._ChampionIdRef, authoring.championPrefabs, thisEntity);
            CreatePrefabData<MonsterId, MonsterPrefabBuffer>(out prefabIdData._MonsterIdRef, authoring.monsterPrefabs, thisEntity);
            CreatePrefabData<TeamType, TowerId, TowerPrefabBuffer>(out prefabIdData._TowerIdRef, authoring.towerPrefabs, thisEntity);
            CreatePrefabData<TeamType, MinionId, MinionPrefabBuffer>(out prefabIdData._MinionIdRef, authoring.minionPrefabs, thisEntity);

            AddComponent(thisEntity, prefabIdData);
        }

        private void CreatePrefabData<TKey, TPrefabBuffer>(
            out BlobAssetReference<BlobMap<EquatableEnum<TKey>, int>> idRef
          , ICovKVPCollection<TKey, GameObject>                       source
          , Entity                                                    thisEntity)
            where TKey : struct, Enum
            where TPrefabBuffer : unmanaged, IPrefabBuffer {
            // INIT PREFAB BUFFER
            var buffer = AddBuffer<TPrefabBuffer>(thisEntity);
            buffer.ResizeUninitialized(source.Count);

            // INIT BUILDER
            var     builder = new BlobBuilder(Allocator.Temp);
            ref var root    = ref builder.ConstructRoot<BlobMap<EquatableEnum<TKey>, int>>();
            int     curId   = -1;

            // BUILD VALUE
            var idBuilder = builder.Allocate(ref root, source.Select(kvp => (EquatableEnum<TKey>)kvp.Key).ToList());
            foreach (var (key, value) in source)
                buffer[idBuilder[key] = ++curId] = new TPrefabBuffer {
                    Entity = GetEntity(value, TransformUsageFlags.Dynamic)
                };

            // GEN REFERENCE AND ADD TO BAKER
            idRef = builder.CreateBlobAssetReference<BlobMap<EquatableEnum<TKey>, int>>(Allocator.Persistent);
            AddBlobAsset(ref idRef, out _);
            builder.Dispose();
        }

        private void CreatePrefabData<TKey1, TKey2, TPrefabBuffer>(
            out BlobAssetReference<BlobMap<EquatableEnum<TKey1>, BlobMap<EquatableEnum<TKey2>, int>>> idRef
          , ICovKVPCollection<TKey1, ICovKVPCollection<TKey2, GameObject>>                            source
          , Entity                                                                                    thisEntity)
            where TKey1 : struct, Enum
            where TKey2 : struct, Enum
            where TPrefabBuffer : unmanaged, IPrefabBuffer {
            // INIT PREFAB BUFFER
            var buffer = AddBuffer<TPrefabBuffer>(thisEntity);
            buffer.ResizeUninitialized(source.Sum(item => item.Value.Count));

            // INIT BUILDER
            var     builder = new BlobBuilder(Allocator.Temp);
            ref var root    = ref builder.ConstructRoot<BlobMap<EquatableEnum<TKey1>, BlobMap<EquatableEnum<TKey2>, int>>>();
            int     curId   = -1;

            // BUILD OUTER VALUE
            var outerIdBuilder = builder.Allocate(ref root
              , source.Select(kvp => (EquatableEnum<TKey1>)kvp.Key).ToList());
            foreach (var (outerKey, outerValue) in source) {
                // BUILD INNER VALUE
                var innerIdBuilder = builder.Allocate(ref outerIdBuilder[outerKey]
                  , outerValue.Select(kvp => (EquatableEnum<TKey2>)kvp.Key).ToList());
                foreach (var (innerKey, innerValue) in outerValue)
                    buffer[innerIdBuilder[innerKey] = ++curId] = new TPrefabBuffer {
                        Entity = GetEntity(innerValue, TransformUsageFlags.Dynamic)
                    };
            }

            // GEN REFERENCE AND ADD TO BAKER
            idRef = builder.CreateBlobAssetReference<BlobMap<EquatableEnum<TKey1>, BlobMap<EquatableEnum<TKey2>, int>>>(Allocator.Persistent);
            AddBlobAsset(ref idRef, out _);
            builder.Dispose();
        }
    }
}