using Unity.Collections;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    internal interface IBlobBuildableString :
        IBlobBuildable<string>
        // use BlobString instead of BubleString because it doesn't need to be nested (so we use the most common type)
      , IBlobBuildableSelf<BlobString> {
        void BuildBlob<TSource>(
            ref BlobBuilder builder
          , ref TSource     source)
            where TSource : INativeList<byte>;
    }
}