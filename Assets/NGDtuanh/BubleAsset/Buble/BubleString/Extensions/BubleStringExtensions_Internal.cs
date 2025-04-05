using Unity.Collections;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    internal static class BubleStringExtensions_Internal {
        public static void AllocateString(
            ref BlobBuilder builder
          , ref BubleString bubleStr
          , string          value)
            => bubleStr.BuildBlob(ref builder, value);

        public static void AllocateString<T>(
            ref BlobBuilder builder
          , ref BubleString bubleStr
          , ref T           value)
            where T : INativeList<byte>
            => bubleStr.BuildBlob(ref builder, ref value);
    }
}