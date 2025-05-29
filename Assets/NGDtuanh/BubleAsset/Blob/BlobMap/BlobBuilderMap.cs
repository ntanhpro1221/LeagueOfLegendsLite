using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace NGDtuanh.BubleAsset {
    /// <summary>
    /// <b>Remember to dispose me</b>
    /// </summary>
    public readonly ref struct BlobBuilderMap<TKey, TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : struct {
        internal readonly BlobBuilderArray<KeyIndex> _KeyIndexes;
        internal readonly BlobBuilderArray<TKey>     _Keys;
        internal readonly BlobBuilderArray<TValue>   _Values;
        internal readonly int                        _Count;

        internal BlobBuilderMap(
            in BlobBuilderArray<KeyIndex> keyIndexes
          , in BlobBuilderArray<TKey>     keys
          , in BlobBuilderArray<TValue>   values
          , in int                        count) {
            _KeyIndexes = keyIndexes;
            _Keys       = keys;
            _Values     = values;
            _Count      = count;
        }

        public ref TValue this[in TKey key] {
            get {
                var keyIndex = _KeyIndexes[BlobMapHasher<TKey>.GetHashedKey(key, _Count)];
                for (int i = keyIndex.first, end = keyIndex.GetLast(); i <= end; ++i)
                    if (_Keys[i].Equals(key))
                        return ref _Values[i];
                
                throw new KeyNotFoundException();
            }
        }
    }
}