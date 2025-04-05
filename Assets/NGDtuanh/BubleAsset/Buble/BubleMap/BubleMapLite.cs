using System;
using System.Collections;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    public struct BubleMap<TKey, TValue> :
        IBlobMapWrapper<TKey, TValue>
      , IBlobBuildable<ICovKVPCollection<TKey, TValue>>
      , IBlobBuildableSelf<BubleMap<TKey, TValue>>
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : struct {
        public BlobMap<TKey, TValue> Value;

        public void BuildBlob(ref BlobBuilder builder, ICovKVPCollection<TKey, TValue> source)
            => builder.SetMapDirectly(ref Value, source);

        public void BuildBlob(ref BlobBuilder builder, ref BubleMap<TKey, TValue> source)
            => builder.SetMapDirectly(ref Value, ref source.Value);

        #region BLOB MAP FUCNTIONS

        public int Count => Value.Count;

        public ref TValue this[TKey key] => ref Value[key];

        public bool ContainsKey(TKey key)                   => Value.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => Value.TryGetValue(key, out value);

        public Dictionary<TKey, TValue>           ToDictionary()  => Value.ToDictionary();
        public IEnumerator<ICovKVP<TKey, TValue>> GetEnumerator() => Value.GetEnumerator();
        IEnumerator IEnumerable.                  GetEnumerator() => GetEnumerator();

        #endregion
    }
}