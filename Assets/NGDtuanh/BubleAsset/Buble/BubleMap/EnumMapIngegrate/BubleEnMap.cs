using System;
using System.Collections;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    public struct BubleEnMap<TKey, TValueResult, TValueSource> :
        IBlobMapWrapper<TKey, TValueResult>
      , IBlobBuildable<ICovKVPCollection<TKey, TValueSource>>
      , IBlobBuildableSelf<BubleEnMap<TKey, TValueResult, TValueSource>>
        where TKey : unmanaged, Enum
        where TValueResult : struct, IBlobBuildable<TValueSource>, IBlobBuildableSelf<TValueResult> {
        public BlobMap<EqualEnum<TKey>, TValueResult> Value;

        public void BuildBlob(ref BlobBuilder builder, ICovKVPCollection<TKey, TValueSource> source)
            => builder.SetMap(ref Value, source.CastKey_EqualEnum());

        public void BuildBlob(ref BlobBuilder builder, ref BubleEnMap<TKey, TValueResult, TValueSource> source)
            => builder.SetMap(ref Value, ref source.Value);

        #region BLOB MAP FUCNTIONS

        public int Count => Value.Count;

        public ref TValueResult this[TKey key] => ref Value[key];

        public bool ContainsKey(TKey key)                         => Value.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValueResult value) => Value.TryGetValue(key, out value);

        public Dictionary<TKey, TValueResult> ToDictionary() => Value.ToDictionary().CastKey_Enum();

        public IEnumerator<ICovKVP<TKey, TValueResult>> GetEnumerator() {
            foreach (var (key, value) in Value)
                yield return new KVPairUnmanaged<TKey, TValueResult>(key, value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}