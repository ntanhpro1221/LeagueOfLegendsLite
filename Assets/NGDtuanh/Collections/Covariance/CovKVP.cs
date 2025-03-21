using System.Collections.Generic;

namespace NGDtuanh.Collections {
    public struct CovKVP<TKey, TValue> : ICovKVP<TKey, TValue> {
        public TKey   Key   { get; set; }
        public TValue Value { get; set; }

        public CovKVP(in TKey key, in TValue value)
            => (Key, Value) = (key, value);

        public CovKVP(in KeyValuePair<TKey, TValue> kvp)
            => (Key, Value) = (kvp.Key, kvp.Value);

        public static implicit operator KeyValuePair<TKey, TValue>(CovKVP<TKey, TValue> kvp)
            => new(kvp.Key, kvp.Value);

        public static implicit operator CovKVP<TKey, TValue>(KeyValuePair<TKey, TValue> kvp)
            => new(kvp);
    }
}