using System;

namespace BlobAssetExtend {
    public struct KeyValueUnmanaged<TKey, TValue> where TKey : unmanaged, IEquatable<TKey>
                                                  where TValue : unmanaged {
        public TKey   Key;
        public TValue Value;

        public KeyValueUnmanaged(in TKey key, in TValue value) {
            Key   = key;
            Value = value;
        }

        public void Deconstruct(out TKey key, out TValue value) {
            key   = Key;
            value = Value;
        }
    }
}