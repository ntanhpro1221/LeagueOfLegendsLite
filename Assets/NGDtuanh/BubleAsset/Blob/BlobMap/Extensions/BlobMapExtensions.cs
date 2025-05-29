using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace NGDtuanh.BubleAsset {
    [BurstCompile]
    public static class BlobMapExtensions {
        #region ALLOCATE

        /// <summary>
        /// Allocate hashmap for you 😘
        /// </summary>
        [BurstCompile]
        public static BlobBuilderMap<TKey, TValue> Allocate<TKey, TValue>(
            this ref BlobBuilder           builder
          , ref      BlobMap<TKey, TValue> ptr
          , in       NativeArray<TKey>     keys)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : struct {

            var bucketSize = keys.Length;

            // create key table
            var keyTable = new NativeArray<NativeList<TKey>>(bucketSize, Allocator.Temp);
            for (int i = 0; i < bucketSize; ++i)
                keyTable[i] = new NativeList<TKey>(Allocator.Temp);
            foreach (var key in keys)
                keyTable[BlobMapHasher<TKey>.GetHashedKey(key, bucketSize)].Add(key);

            // create firstKeyTable
            var firstKeyTable = new NativeArray<int>(bucketSize, Allocator.Temp);
            for (int i = 1; i < bucketSize; ++i)
                firstKeyTable[i] += firstKeyTable[i - 1] + keyTable[i - 1].Length;

            var result = new BlobBuilderMap<TKey, TValue>(
                builder.Allocate(ref ptr._KeyIndexes, bucketSize)
              , builder.Allocate(ref ptr.Keys,        bucketSize)
              , builder.Allocate(ref ptr.Values,      bucketSize)
              , bucketSize);
            
            // create key indexes
            foreach (var key in keys) {
                var hashedKey = BlobMapHasher<TKey>.GetHashedKey(key, bucketSize);
                result._KeyIndexes[hashedKey] = new(firstKeyTable[hashedKey], keyTable[hashedKey].Length);
            }

            // create flat key table
            int flatKeyTable_Id = 0;
            foreach (var keyBucketElement in keyTable)
            foreach (var keyElement in keyBucketElement)
                result._Keys[flatKeyTable_Id++] = keyElement;

            // Dispose
            firstKeyTable.Dispose();
            for (int i = 0; i < bucketSize; ++i)
                keyTable[i].Dispose();
            keyTable.Dispose();

            return result;
        }

        public static BlobBuilderMap<TKey, TValue> Allocate<TKey, TValue>(
            this ref BlobBuilder           builder
          , ref      BlobMap<TKey, TValue> ptr
          , IReadOnlyCollection<TKey>      keys)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : struct {

            var keysNativeArray = new NativeArray<TKey>(keys.ToArray(), Allocator.Temp);

            var result = builder.Allocate(ref ptr, keysNativeArray);

            keysNativeArray.Dispose();

            return result;
        }

        [BurstCompile]
        public static BlobBuilderMap<TKey, TValue> Allocate<TKey, TValue>(
            this ref BlobBuilder           builder
          , ref      BlobMap<TKey, TValue> ptr
          , ref      BlobArray<TKey>       keys)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : struct {

            var keysNativeArray = new NativeArray<TKey>(keys.Length, Allocator.Temp);
            for (int i = 0; i < keys.Length; ++i)
                keysNativeArray[i] = keys[i];

            var result = builder.Allocate(ref ptr, keysNativeArray);

            keysNativeArray.Dispose();

            return result;
        }

        #endregion
    }
}