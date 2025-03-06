using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

namespace BlobAssetExtend {
    public struct BlobHashMap<TKey, TValue> : IEnumerable<KeyValueUnmanaged<TKey, TValue>>
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged {
        internal BlobArray<BlobArray<KeyValueUnmanaged<TKey, TValue>>> _Buckets;
        public   int                                                   Count { get; internal set; }

        public TValue this[TKey key] {
            get {
                ref var bucketElement     = ref _Buckets[GetHashedKey(key)];
                var     bucketElementSize = bucketElement.Length;
                for (int i = 0; i < bucketElementSize; ++i)
                    if (bucketElement[i].Key.Equals(key))
                        return bucketElement[i].Value;
                throw new KeyNotFoundException();
            }
        }

        public IEnumerator<KeyValueUnmanaged<TKey, TValue>> GetEnumerator() {
            for (int i = 0, bucketSize = Count; i < bucketSize; ++i)
            for (int j = 0, bucketElementSize = _Buckets[i].Length; j < bucketElementSize; ++j)
                yield return _Buckets[i][j];
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private readonly int GetHashedKey(in TKey key) => GetHashedKey(key, Count);

        internal static int GetHashedKey(in TKey key, in int count) {
            return (int)((ulong)key.GetHashCode() * 11400714819323198485ul) % count;
        }
    }
}