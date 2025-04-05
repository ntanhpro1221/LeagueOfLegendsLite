using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    public interface IBlobBuildableSelf<TSelf> {
        void BuildBlob(
            ref BlobBuilder builder
          , ref TSelf       source);
    }
}