using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    public interface IBlobBuildable<in TSource> {
        void BuildBlob(
            ref BlobBuilder builder
          , TSource         source);
    }
}