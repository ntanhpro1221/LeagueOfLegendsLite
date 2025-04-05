using System;
using System.Collections;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    public struct BlobMap<TKey, TValue> :
        IBlobMapWrapper<TKey, TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : struct {
        internal BlobArray<KeyIndex> _KeyIndexes;
        public   BlobArray<TKey>     Keys;
        public   BlobArray<TValue>   Values;
        public   int                 Count => Keys.Length;

        public ref TValue this[TKey key] {
            get {
                ref var keyIndex = ref _KeyIndexes[GetHashedKey(key)];
                for (int i = keyIndex.first, end = keyIndex.GetLast(); i <= end; ++i)
                    if (Keys[i].Equals(key))
                        return ref Values[i];

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
                result.Add(Keys[i], Values[i]);
            return result;
        }

        public IEnumerator<ICovKVP<TKey, TValue>> GetEnumerator() {
            for (int i = 0; i < Count; ++i)
                yield return new KVPairUnmanaged<TKey, TValue>(Keys[i], Values[i]);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private int GetHashedKey(TKey key) => GetHashedKey(key, Count);

        internal static int GetHashedKey(TKey key, int count) {
            return (int)(((ulong)key.GetHashCode() * 11400714819323198485ul) >> 33) % count;
        }
    }
}