using System;
using System.Collections.Generic;
using Unity.Entities;

namespace BlobAssetExtend {
    public static class BlobBuilderExtensions {
        public static void SetArray<T>(this BlobBuilder  builder
                                     , ref  BlobArray<T> array
                                     , ICollection<T>    source) where T : struct {

            var size = source.Count;

            var arrayBuilder = builder.Allocate(ref array, size);
            int curId        = 0;
            foreach (var value in source)
                arrayBuilder[curId++] = value;
        }

        public static void SetHashMap<TKey, TValue>(this BlobBuilder                             builder
                                                  , ref  BlobHashMap<TKey, TValue>               hashMap
                                                  , ICollection<KeyValueUnmanaged<TKey, TValue>> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged {

            var bucketSize = source.Count;

            // build hash table
            var hashTablePattern = new List<KeyValueUnmanaged<TKey, TValue>>[bucketSize];
            for (int i = 0; i < bucketSize; ++i)
                hashTablePattern[i] = new();

            foreach (var (key, value) in source)
                hashTablePattern[BlobHashMap<TKey, TValue>.GetHashedKey(key, bucketSize)].Add(new(key, value));

            hashMap.Count = bucketSize;
            var bucketBuilder = builder.Allocate(ref hashMap._Buckets, bucketSize);
            for (int i = 0; i < bucketSize; ++i) {
                var elementSize    = hashTablePattern[i].Count;
                var elementBuilder = builder.Allocate(ref bucketBuilder[i], elementSize);
                for (int j = 0; j < elementSize; ++j)
                    elementBuilder[j] = hashTablePattern[i][j];
            }
        }
    }
}