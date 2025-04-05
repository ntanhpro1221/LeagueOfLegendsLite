using System;
using System.Collections;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    public struct BubleEnMap<TKey, TValue> :
        IBlobMapWrapper<TKey, TValue>
      , IBlobBuildable<ICovKVPCollection<TKey, TValue>>
      , IBlobBuildableSelf<BubleEnMap<TKey, TValue>>
        where TKey : unmanaged, Enum
        where TValue : struct {
        public BlobMap<EqualEnum<TKey>, TValue> Value;

        public void BuildBlob(ref BlobBuilder builder, ICovKVPCollection<TKey, TValue> source)
            => builder.SetMapDirectly(ref Value, source.CastKey_EqualEnum());

        public void BuildBlob(ref BlobBuilder builder, ref BubleEnMap<TKey, TValue> source)
            => builder.SetMapDirectly(ref Value, ref source.Value);

        #region BLOB MAP FUCNTIONS

        public int Count => Value.Count;

        public ref TValue this[TKey key] => ref Value[key];

        public bool ContainsKey(TKey key)                   => Value.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => Value.TryGetValue(key, out value);

        public Dictionary<TKey, TValue> ToDictionary() => Value.ToDictionary().CastKey_Enum();

        public IEnumerator<ICovKVP<TKey, TValue>> GetEnumerator() {
            foreach (var (key, value) in Value)
                yield return new KVPairUnmanaged<TKey, TValue>(key, value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}