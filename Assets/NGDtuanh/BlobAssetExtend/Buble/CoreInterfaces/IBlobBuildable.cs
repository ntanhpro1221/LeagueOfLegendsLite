using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    public interface IBlobBuildable<in TSource> {
        void BuildBlob(
            ref BlobBuilder builder
          , TSource         source);
    }
}