using System.Collections.Generic;
using NGDtuanh.Collections;

namespace NGDtuanh.BlobAssetExtend {
    internal interface IBlobMapWrapper<TKey, TValue> :
        IReadOnlyCollection<KVPairUnmanaged<TKey, TValue>>
        where TKey : struct
        where TValue : struct {
        ref TValue this[TKey key] { get; }

        bool ContainsKey(TKey key);

        bool TryGetValue(TKey key, out TValue value);

        Dictionary<TKey, TValue> ToDictionary();
    }
}