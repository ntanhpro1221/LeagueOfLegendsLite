using System.Collections.Generic;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    internal static class BlobArrayHelper_Internal {
        #region SET VALUE

        /// <summary>
        /// When you already have builder and constructed root of BlobArray. <br/>
        /// It just helps you allocate and then loops through source and set value to your root.
        /// </summary>
        public static void SetArray<T>(
            this ref BlobBuilder   builder
          , ref      BlobArray<T>  array
          , IReadOnlyCollection<T> source
          , IBaker                 _ = null)
            where T : struct {

            var size         = source.Count;
            var arrayBuilder = builder.Allocate(ref array, size);
            int curId        = -1;
            foreach (var value in source)
                arrayBuilder[++curId] = value;
        }

        public static void SetArray<TResult, TSource>(
            this ref BlobBuilder         builder
          , ref      BlobArray<TResult>  array
          , IReadOnlyCollection<TSource> source
          , IBaker                       baker)
            where TResult : struct, IBlobBuildable<TSource> {

            var size         = source.Count;
            var arrayBuilder = builder.Allocate(ref array, size);
            int curId        = -1;
            foreach (var sourceValue in source)
                arrayBuilder[++curId].BuildBlob(ref builder, sourceValue, baker);
        }

        #endregion
    }
}