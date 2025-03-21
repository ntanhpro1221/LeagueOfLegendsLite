using System;
using System.Collections;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    public struct BubleEnMap<TKey, TValue> :
        IBlobMapWrapper<TKey, TValue>
      , IBlobBuildable<ICovKVPCollection<TKey, TValue>>
        where TKey : struct, Enum
        where TValue : struct {
        public BlobMap<EquatableEnum<TKey>, TValue> Value;

        public void BuildBlob(ref BlobBuilder builder, ICovKVPCollection<TKey, TValue> source, IBaker baker)
            => builder.SetMap(ref Value, source.CastKey_EqualEnum(), baker);

        #region BLOB MAP FUCNTIONS

        public int Count => Value.Count;

        public ref TValue this[TKey key] => ref Value[key];

        public bool ContainsKey(TKey key)                   => Value.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => Value.TryGetValue(key, out value);

        public Dictionary<TKey, TValue> ToDictionary() => Value.ToDictionary().CastKey_Enum();

        public IEnumerator<KVPairUnmanaged<TKey, TValue>> GetEnumerator() {
            foreach (var (key, value) in Value)
                yield return new(key, value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}