using System.Collections.Generic;
using NGDtuanh.Collections;

namespace AYellowpaper.SerializedCollections {
    [System.Serializable]
    public class CovSerializedDictionary<TKey, TValue> :
        SerializedDictionary<TKey, TValue>
      , ICovKVPCollection<TKey, TValue> {
        IEnumerator<ICovKVP<TKey, TValue>> IEnumerable<ICovKVP<TKey, TValue>>.GetEnumerator() {
            foreach (var kvp in this)
                yield return new CovKVP<TKey, TValue>(kvp);
        }
    }
}