using System;
using System.Collections;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    public struct BlobMap<TKey, TValue> : IBlobMapWrapper<TKey, TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : struct {
        private  BlobArray<KeyIndex> _KeyIndexes;
        private  BlobArray<TKey>     _Keys;
        internal BlobArray<TValue>   _Values;
        public   int                 Count { get; private set; }

        internal void BuildKeyTable(
            ref BlobBuilder                   builder
          , in  IReadOnlyCollection<KeyIndex> keyIndexes
          , in  IReadOnlyCollection<TKey>     keys
          , in  int                           count) {
            builder.SetArray(ref _KeyIndexes, keyIndexes);
            builder.SetArray(ref _Keys,       keys);
            Count = count;
        }

        public ref TValue this[TKey key] {
            get {
                ref var keyIndex = ref _KeyIndexes[GetHashedKey(key)];
                for (int i = keyIndex.first, end = keyIndex.GetLast(); i <= end; ++i)
                    if (_Keys[i].Equals(key))
                        return ref _Values[i];
                throw new KeyNotFoundException();
            }
        }

        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        public bool TryGetValue(TKey key, out TValue value) {
            try {
                value = this[key];
                return true;
            }
            catch {
                value = default;
                return false;
            }
        }

        public Dictionary<TKey, TValue> ToDictionary() {
            Dictionary<TKey, TValue> result = new();
            for (int i = 0; i < Count; ++i)
                result.Add(_Keys[i], _Values[i]);
            return result;
        }

        public IEnumerator<KVPairUnmanaged<TKey, TValue>> GetEnumerator() {
            for (int i = 0; i < Count; ++i)
                yield return new KVPairUnmanaged<TKey, TValue>(_Keys[i], _Values[i]);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private readonly int GetHashedKey(TKey key) => GetHashedKey(key, Count);

        internal static int GetHashedKey(TKey key, int count) {
            return (int)(((ulong)key.GetHashCode() * 11400714819323198485ul) >> 33) % count;
        }
    }
}