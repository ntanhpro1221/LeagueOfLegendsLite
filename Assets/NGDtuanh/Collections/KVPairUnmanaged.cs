using System.Collections.Generic;

namespace NGDtuanh.Collections {
    public struct KVPairUnmanaged<TKey, TValue> :
        ICovKVP<TKey, TValue>
        where TKey : struct
        where TValue : struct {
        public TKey   Key   { get; set; }
        public TValue Value { get; set; }

        public KVPairUnmanaged(in TKey key, in TValue value)
            => (Key, Value) = (key, value);

        public static implicit operator KeyValuePair<TKey, TValue>(in KVPairUnmanaged<TKey, TValue> pair)
            => new(pair.Key, pair.Value);

        public static implicit operator KVPairUnmanaged<TKey, TValue>(in KeyValuePair<TKey, TValue> pair)
            => new(pair.Key, pair.Value);

        public void Deconstruct(out TKey key, out TValue value)
            => (key, value) = (Key, Value);
    }
}