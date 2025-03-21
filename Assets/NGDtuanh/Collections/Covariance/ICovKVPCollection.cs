using System.Collections.Generic;

namespace NGDtuanh.Collections {
    public interface ICovKVPCollection<out TKey, out TValue> : IReadOnlyCollection<ICovKVP<TKey, TValue>> { }
}