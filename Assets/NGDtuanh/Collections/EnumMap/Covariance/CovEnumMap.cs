using System;
using System.Collections.Generic;

namespace NGDtuanh.Collections {
    [Serializable]
    public class CovEnumMap<TKey, TValue> :
        EnumMap<TKey, TValue>
      , ICovKVPCollection<TKey, TValue>
        where TKey : struct, Enum {
        IEnumerator<ICovKVP<TKey, TValue>> IEnumerable<ICovKVP<TKey, TValue>>.GetEnumerator() {
            foreach (var kvp in this)
                yield return new CovKVP<TKey, TValue>(kvp);
        }
    }
}