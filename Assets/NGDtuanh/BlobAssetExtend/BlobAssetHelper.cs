using Unity.Collections;
using Unity.Entities;
using Hash128 = Unity.Entities.Hash128;

namespace NGDtuanh.BlobAssetExtend {
    public static class BlobAssetHelper {
        public static void CreateBlobAssetReference<TResult>(
            this TResult                     source
          , out  BlobAssetReference<TResult> result)
            where TResult : unmanaged {

            var     builder = new BlobBuilder(Allocator.Temp);
            ref var root    = ref builder.ConstructRoot<TResult>();
            root   = source;
            result = builder.CreateBlobAssetReference<TResult>(Allocator.Persistent);
            builder.Dispose();
        }

        public static void CreateBlobAssetReferenceInBaker<TResult>(
            this TResult                     source
          , out  BlobAssetReference<TResult> result
          , IBaker                           baker
          , out Hash128                      hash)
            where TResult : unmanaged {
            source.CreateBlobAssetReference(out result);
            baker.AddBlobAsset(ref result, out hash);
        }

        public static void CreateBlobAssetReference<TResult, TSource>(
            this TSource                     source
          , out  BlobAssetReference<TResult> result
          , IBaker                           baker)
            where TResult : unmanaged, IBlobBuildable<TSource> {

            var     builder = new BlobBuilder(Allocator.Temp);
            ref var root    = ref builder.ConstructRoot<TResult>();
            root.BuildBlob(ref builder, source, baker);
            result = builder.CreateBlobAssetReference<TResult>(Allocator.Persistent);
            builder.Dispose();
        }

        public static void CreateBlobAssetReferenceInBaker<TResult, TSource>(
            this TSource                     source
          , out  BlobAssetReference<TResult> result
          , IBaker                           baker
          , out Hash128                      hash)
            where TResult : unmanaged, IBlobBuildable<TSource> {

            source.CreateBlobAssetReference(out result, baker);
            baker.AddBlobAsset(ref result, out hash);
        }
    }
}