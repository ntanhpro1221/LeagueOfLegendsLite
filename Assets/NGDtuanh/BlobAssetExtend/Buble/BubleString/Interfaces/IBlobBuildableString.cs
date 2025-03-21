using Unity.Collections;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    internal interface IBlobBuildableString : IBlobBuildable<string> {
        void BuildBlob<TSource>(
            ref BlobBuilder builder
          , ref TSource     source)
            where TSource : INativeList<byte>;
    }
}