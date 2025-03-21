using Unity.Collections;

namespace NGDtuanh.BlobAssetExtend {
    internal interface IBlobStringWrapper {
        int             Length { get; }
        string          ToString();
        ConversionError CopyTo<T>(ref T dest) where T : INativeList<byte>;
    }
}