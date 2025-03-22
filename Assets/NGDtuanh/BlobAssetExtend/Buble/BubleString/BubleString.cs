using Unity.Collections;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    public struct BubleString : IBlobStringWrapper, IBlobBuildableString {
        public BlobString Value;

        public void BuildBlob(ref BlobBuilder builder, string source) {
            builder.AllocateString(ref Value, source);
        }

        public void BuildBlob<TSource>(ref BlobBuilder builder, ref TSource source) where TSource : INativeList<byte> {
            builder.AllocateString(ref Value, ref source);
        }

        #region BLOB STRING WRAPPER

        public     int    Length     => Value.Length;
        public new string ToString() => Value.ToString(); // because BlobString also use "new" instead of "override"

        public ConversionError CopyTo<T>(ref T dest)
            where T : INativeList<byte>
            => Value.CopyTo(ref dest);

        #endregion
    }
}