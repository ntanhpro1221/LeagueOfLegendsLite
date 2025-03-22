using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    public static class BlobMapExtensions {
        #region ALLOCATE

        /// <summary>
        /// Allocate hashmap for you 😘
        /// </summary>
        public static BlobBuilderMap<TKey, TValue> Allocate<TKey, TValue>(
            this ref BlobBuilder           builder
          , ref      BlobMap<TKey, TValue> ptr
          , IReadOnlyCollection<TKey>      rawKeys)
            where TKey : struct, IEquatable<TKey>
            where TValue : struct {

            var keys       = rawKeys.Distinct().ToList();
            var bucketSize = keys.Count;

            // create key table
            var keyTable = new List<TKey>[bucketSize];
            for (int i = 0; i < bucketSize; ++i)
                keyTable[i] = new();
            foreach (var key in keys)
                keyTable[BlobMap<TKey, TValue>.GetHashedKey(key, bucketSize)].Add(key);

            // create firstKeyTable
            var firstKeyTable = new int[bucketSize];
            for (int i = 1; i < bucketSize; ++i)
                firstKeyTable[i] += firstKeyTable[i - 1] + keyTable[i - 1].Count;

            // create key indexes
            var keyIndexes = new KeyIndex[bucketSize];
            foreach (var key in keys) {
                var hashedKey = BlobMap<TKey, TValue>.GetHashedKey(key, bucketSize);
                keyIndexes[hashedKey] = new(firstKeyTable[hashedKey], keyTable[hashedKey].Count);
            }

            // create flat key table
            var flatKeyTable    = new TKey[bucketSize];
            int flatKeyTable_Id = 0;
            foreach (var keyBucketElement in keyTable)
            foreach (var keyElement in keyBucketElement)
                flatKeyTable[flatKeyTable_Id++] = keyElement;

            // build key table in hashmap
            ptr.BuildKeyTable(ref builder, keyIndexes, flatKeyTable, bucketSize);

            return new(
                keyIndexes
              , flatKeyTable
              , builder.Allocate(ref ptr.Values, bucketSize)
              , bucketSize);
        }

        #endregion
    }
}