using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace BlobAssetExtend {
    public static class BlobBuilderExtensions {
        public static void SetArray<T>(this BlobBuilder  builder
                                     , ref  BlobArray<T> array
                                     , ICollection<T>    source)
            where T : struct {

            var size = source.Count;

            var arrayBuilder = builder.Allocate(ref array, size);
            int curId        = 0;
            foreach (var value in source)
                arrayBuilder[curId++] = value;
        }

        public static void SetHashMap<TKey, TValue>(this BlobBuilder                        builder
                                                  , ref  BlobHashMap<TKey, TValue>          hashMap
                                                  , ICollection<KeyValuePair<TKey, TValue>> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged {
            var hashMapBuilder = builder.Allocate(ref hashMap, source.Select(item => item.Key).ToList());
            foreach (var (key, value) in source)
                hashMapBuilder[key] = value;
        }

        public static void SetHashMap<TKey, TValue, TManagedValue>(this BlobBuilder                        builder
                                                                   , IBaker baker
                                                                 , ref  BlobHashMap<TKey, TValue>          hashMap
                                                                 , ICollection<KeyValuePair<TKey, TManagedValue>> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged, IConstructableFromOtherVersion<TManagedValue> {
            var hashMapBuilder = builder.Allocate(ref hashMap, source.Select(item => item.Key).ToList());
            foreach (var (key, value) in source)
                hashMapBuilder[key].Construct(builder, baker, value);
        }

        public static BlobBuilderHashMap<TKey, TValue> Allocate<TKey, TValue>(this BlobBuilder               builder
                                                                            , ref  BlobHashMap<TKey, TValue> hashMap
                                                                            , ICollection<TKey>              keys)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged {

            var bucketSize = keys.Count;

            // create key table
            var keyTable = new List<TKey>[bucketSize];
            for (int i = 0; i < bucketSize; ++i)
                keyTable[i] = new();
            foreach (var key in keys)
                keyTable[BlobHashMap<TKey, TValue>.GetHashedKey(key, bucketSize)].Add(key);

            // create firstKeyTable
            var firstKeyTable = new int[bucketSize];
            for (int i = 1; i < bucketSize; ++i)
                firstKeyTable[i] += firstKeyTable[i - 1] + keyTable[i - 1].Count;

            // create key indexes
            var keyIndexes = new KeyIndex[bucketSize];
            foreach (var key in keys) {
                var hashedKey = BlobHashMap<TKey, TValue>.GetHashedKey(key, bucketSize);
                keyIndexes[hashedKey] = new(firstKeyTable[hashedKey], keyTable[hashedKey].Count);
            }

            // create flat key table
            var flatKeyTable    = new TKey[bucketSize];
            int flatKeyTable_Id = 0;
            foreach (var keyBucketElement in keyTable)
            foreach (var keyElement in keyBucketElement)
                flatKeyTable[flatKeyTable_Id++] = keyElement;

            // build key table in hash map
            hashMap.BuildKeyTable(builder, keyIndexes, flatKeyTable, bucketSize);

            return new(
                keyIndexes
              , flatKeyTable
              , builder.Allocate(ref hashMap._Values, bucketSize)
              , bucketSize);
        }
    }
}