using System.Collections.Generic;

namespace NGDtuanh.Collections {
    public static class CovKVPExtensions {
        public static void Deconstruct<TKey, TValue>(
            this ICovKVP<TKey, TValue> kvp
          , out  TKey                  key
          , out  TValue                value)
            => (key, value) = (kvp.Key, kvp.Value);
    }
}