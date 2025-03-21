using System;
using System.Linq;
using NGDtuanh.Collections;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    internal static class BlobMapHelper_Internal { 
        #region SET VALUE

        /// <summary>
        /// When you already have builder and constructed root of BlobHashMap. <br/>
        /// It just helps you allocate and then loops through source and set value to your root.
        /// </summary>
        public static void SetMap<TKey, TValue>(
            this ref BlobBuilder            builder
          , ref      BlobMap<TKey, TValue>  map
          , ICovKVPCollection<TKey, TValue> source
          , IBaker                          _ = null)
            where TKey : struct, IEquatable<TKey>
            where TValue : struct {

            var hashMapBuilder = builder.Allocate(ref map, source.Select(item => item.Key).ToList());
            foreach (var (sourceKey, sourceValue) in source)
                hashMapBuilder[sourceKey] = sourceValue;
        }

        public static void SetMap<TKey, TValueResult, TValueSource>(
            this ref BlobBuilder                  builder
          , ref      BlobMap<TKey, TValueResult>  map
          , ICovKVPCollection<TKey, TValueSource> source
          , IBaker                                baker)
            where TKey : struct, IEquatable<TKey>
            where TValueResult : struct, IBlobBuildable<TValueSource> {

            var hashMapBuilder = builder.Allocate(ref map, source.Select(item => item.Key).ToList());
            foreach (var (sourceKey, sourceValue) in source)
                hashMapBuilder[sourceKey].BuildBlob(ref builder, sourceValue, baker);
        }

        #endregion
    }
}