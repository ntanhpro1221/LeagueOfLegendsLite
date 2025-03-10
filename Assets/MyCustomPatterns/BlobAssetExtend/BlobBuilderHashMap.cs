using System;
using System.Collections.Generic;
using Unity.Entities;

namespace BlobAssetExtend {
    public readonly ref struct BlobBuilderHashMap<TKey, TValue> where TKey : unmanaged, IEquatable<TKey>
                                                                where TValue : unmanaged {
        private readonly KeyIndex[]               _KeyIndexes;
        private readonly TKey[]                   _Keys;
        private readonly BlobBuilderArray<TValue> _Values;
        private readonly int                      _Count;

        internal BlobBuilderHashMap(in KeyIndex[]               keyIndexes
                                  , in TKey[]                   keys
                                  , in BlobBuilderArray<TValue> values
                                  , in int                      count) {
            _KeyIndexes = keyIndexes;
            _Keys       = keys;
            _Values     = values;
            _Count      = count;
        }

        public ref TValue this[in TKey key] {
            get {
                ref var keyIndex = ref _KeyIndexes[BlobHashMap<TKey, TValue>.GetHashedKey(key, _Count)];
                for (int i = keyIndex.first, end = keyIndex.GetLast(); i <= end; ++i)
                    if (_Keys[i].Equals(key))
                        return ref _Values[i];
                throw new KeyNotFoundException();
            }
        }
    }
}