using System;
using System.Linq;
using NGDtuanh.Collections;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace NGDtuanh.BubleAsset {
    [BurstCompile]
    internal static class BlobMapHelper_Internal {
        #region SET VALUE

        /// <summary>
        /// When you already have builder and constructed root of BlobHashMap. <br/>
        /// It just helps you allocate and then loops through source and set value to your root.
        /// </summary>
        public static void SetMap<TKey, TValue>(
            this ref BlobBuilder           builder
          , ref      BlobMap<TKey, TValue> map
          , ref      BlobMap<TKey, TValue> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : struct, IBlobBuildableSelf<TValue> {

            var sourceKeys     = source.Select(item => item.Key).ToList();
            var hashMapBuilder = builder.Allocate(ref map, sourceKeys);
            foreach (var key in sourceKeys)
                hashMapBuilder[key].BuildBlob(ref builder, ref source[key]);
        }

        public static void SetMap<TKey, TValueResult, TValueSource>(
            this ref BlobBuilder                  builder
          , ref      BlobMap<TKey, TValueResult>  map
          , ICovKVPCollection<TKey, TValueSource> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValueResult : struct, IBlobBuildable<TValueSource> {

            var hashMapBuilder = builder.Allocate(ref map, source.Select(item => item.Key).ToList());
            foreach (var (sourceKey, sourceValue) in source)
                hashMapBuilder[sourceKey].BuildBlob(ref builder, sourceValue);
        }

        [BurstCompile]
        public static void SetMapDirectly<TKey, TValue>(
            this ref BlobBuilder           builder
          , ref      BlobMap<TKey, TValue> map
          , ref      BlobMap<TKey, TValue> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : struct {

            var hashMapBuilder = builder.Allocate(ref map, ref source.Keys);
            for (int i = 0; i < source.Count; ++i)
                hashMapBuilder[source.Keys[i]] = source[source.Keys[i]];
        }

        public static void SetMapDirectly<TKey, TValue>(
            this ref BlobBuilder            builder
          , ref      BlobMap<TKey, TValue>  map
          , ICovKVPCollection<TKey, TValue> source)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : struct {

            var hashMapBuilder = builder.Allocate(ref map, source.Select(item => item.Key).ToList());
            foreach (var (sourceKey, sourceValue) in source)
                hashMapBuilder[sourceKey] = sourceValue;
        }

        #endregion
    }
}