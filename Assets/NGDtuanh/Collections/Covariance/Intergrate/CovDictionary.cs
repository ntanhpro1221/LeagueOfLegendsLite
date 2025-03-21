using System.Collections.Generic;

namespace NGDtuanh.Collections {
    public class CovDictionary<TKey, TValue> :
        Dictionary<TKey, TValue>
      , ICovKVPCollection<TKey, TValue> {
        public CovDictionary(Dictionary<TKey, TValue> source) : base(source) { }

        IEnumerator<ICovKVP<TKey, TValue>> IEnumerable<ICovKVP<TKey, TValue>>.GetEnumerator() {
            foreach (var kvp in this)
                yield return new CovKVP<TKey, TValue>(kvp);
        }
    }
}