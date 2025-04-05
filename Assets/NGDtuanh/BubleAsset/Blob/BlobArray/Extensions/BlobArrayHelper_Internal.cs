using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    [BurstCompile]
    internal static class BlobArrayHelper_Internal {
        #region SET VALUE

        /// <summary>
        /// When you already have builder and constructed root of BlobArray. <br/>
        /// It just helps you allocate and then loops through source and set value to your root.
        /// </summary>
        public static void SetArray<T>(
            this ref BlobBuilder  builder
          , ref      BlobArray<T> array
          , ref      BlobArray<T> source)
            where T : struct, IBlobBuildableSelf<T> {

            var arrayBuilder = builder.Allocate(ref array, source.Length);
            for (int i = 0; i < source.Length; ++i)
                arrayBuilder[i].BuildBlob(ref builder, ref source[i]);
        }

        public static void SetArray<TResult, TSource>(
            this ref BlobBuilder         builder
          , ref      BlobArray<TResult>  array
          , IReadOnlyCollection<TSource> source)
            where TResult : struct, IBlobBuildable<TSource> {

            var arrayBuilder = builder.Allocate(ref array, source.Count);
            int curId        = -1;
            foreach (var sourceValue in source)
                arrayBuilder[++curId].BuildBlob(ref builder, sourceValue);
        }
        
        [BurstCompile]
        public static void SetArrayDirectly<T>(
            this ref BlobBuilder    builder
          , ref      BlobArray<T>   array
          , in       NativeArray<T> source)
            where T : struct {

            var arrayBuilder = builder.Allocate(ref array, source.Length);
            for (int i = 0; i < source.Length; ++i)
                arrayBuilder[i] = source[i];
        }

        #endregion

        [BurstCompile]
        public static void SetArrayDirectly<T>(
            this ref BlobBuilder  builder
          , ref      BlobArray<T> array
          , ref      BlobArray<T> source)
            where T : struct {

            var nativeSource = new NativeArray<T>(source.Length, Allocator.Temp);
            for (int i = 0; i < source.Length; ++i)
                nativeSource[i] = source[i];
            
            builder.SetArrayDirectly(ref array, nativeSource);
            
            nativeSource.Dispose();
        }

        public static void SetArrayDirectly<T>(
            this ref BlobBuilder   builder
          , ref      BlobArray<T>  array
          , IReadOnlyCollection<T> source)
            where T : struct {
            var nativeSource = new NativeArray<T>(source.ToArray(), Allocator.Temp);

            builder.SetArrayDirectly(ref array, nativeSource);

            nativeSource.Dispose();
        }
    }
}