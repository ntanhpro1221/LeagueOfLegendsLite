using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    [BurstCompile]
    public static class BlobAssetHelper {
        #region CORE

        public static void CreateBlobAssetReference<TResult, TSource>(
            this TSource                     source
          , out  BlobAssetReference<TResult> result)
            where TResult : unmanaged, IBlobBuildable<TSource> {

            var builder = new BlobBuilder(Allocator.Temp);
            builder.ConstructRoot<TResult>().BuildBlob(ref builder, source);
            result = builder.CreateBlobAssetReference<TResult>(Allocator.Persistent);
            builder.Dispose();
        }

        public static void CreateBlobAssetReferenceInBaker<TResult, TSource>(
            this TSource                     source
          , out  BlobAssetReference<TResult> result
          , IBaker                           baker
          , out Hash128                      hash)
            where TResult : unmanaged, IBlobBuildable<TSource> {

            source.CreateBlobAssetReference(out result);
            baker.AddBlobAsset(ref result, out hash);
        }

        [BurstCompile]
        public static void CreateBlobAssetReference<T>(
            this ref T                     source
          , out      BlobAssetReference<T> result)
            where T : unmanaged, IBlobBuildableSelf<T> {

            var builder = new BlobBuilder(Allocator.Temp);
            builder.ConstructRoot<T>().BuildBlob(ref builder, ref source);
            result = builder.CreateBlobAssetReference<T>(Allocator.Persistent);
            builder.Dispose();
        }

        public static void CreateBlobAssetReferenceInBaker<T>(
            this ref T                     source
          , out      BlobAssetReference<T> result
          , IBaker                         baker
          , out Hash128                    hash)
            where T : unmanaged, IBlobBuildableSelf<T> {

            source.CreateBlobAssetReference(out result);
            baker.AddBlobAsset(ref result, out hash);
        }

        #endregion
    }
}