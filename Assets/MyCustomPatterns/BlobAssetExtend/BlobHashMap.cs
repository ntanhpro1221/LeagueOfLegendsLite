using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace BlobAssetExtend {
    public struct BlobHashMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged {
        private  BlobArray<KeyIndex> _KeyIndexes;
        private  BlobArray<TKey>     _Keys;
        internal BlobArray<TValue>   _Values;
        public   int                 Count { get; private set; }

        internal void BuildKeyTable(BlobBuilder              builder
                                  , in ICollection<KeyIndex> keyIndexes
                                  , in ICollection<TKey>     keys
                                  , in int                   count) {
            builder.SetArray(ref _KeyIndexes, keyIndexes);
            builder.SetArray(ref _Keys,       keys);
            Count = count;
        }

        public ref TValue this[in TKey key] {
            get {
                ref var keyIndex = ref _KeyIndexes[GetHashedKey(key)];
                for (int i = keyIndex.first, end = keyIndex.GetLast(); i <= end; ++i)
                    if (_Keys[i].Equals(key))
                        return ref _Values[i];
                throw new KeyNotFoundException();
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            for (int i = 0; i < Count; ++i)
                yield return new(_Keys[i], _Values[i]);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private readonly int GetHashedKey(in TKey key) => GetHashedKey(key, Count);

        internal static int GetHashedKey(in TKey key, in int count) {
            return (int)(((ulong)key.GetHashCode() * 11400714819323198485ul) >> 33) % count;
        }
    }
}