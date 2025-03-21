using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    internal static class BubleArrayExtensions_Internal {
        public static BlobBuilderArray<TResult> Allocate<TResult, TSource>(
            this BlobBuilder                  builder
          , ref  BubleArray<TResult, TSource> ptr
          , int                               length)
            where TResult : struct, IBlobBuildable<TSource>
            => builder.Allocate(ref ptr.Value, length);
    }
}