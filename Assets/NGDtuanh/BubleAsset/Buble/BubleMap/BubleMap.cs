using System;
using System.Collections;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    public struct BubleMap<TKey, TValueResult, TValueSource> :
        IBlobMapWrapper<TKey, TValueResult>
      , IBlobBuildable<ICovKVPCollection<TKey, TValueSource>>
      , IBlobBuildableSelf<BubleMap<TKey, TValueResult, TValueSource>>
        where TKey : unmanaged, IEquatable<TKey>
        where TValueResult : struct, IBlobBuildable<TValueSource>, IBlobBuildableSelf<TValueResult> {
        public BlobMap<TKey, TValueResult> Value;

        public void BuildBlob(ref BlobBuilder builder, ICovKVPCollection<TKey, TValueSource> source)
            => builder.SetMap(ref Value, source);

        public void BuildBlob(ref BlobBuilder builder, ref BubleMap<TKey, TValueResult, TValueSource> source)
            => builder.SetMap(ref Value, ref source.Value);

        #region BLOB MAP FUCNTIONS

        public int Count => Value.Count;

        public ref TValueResult this[TKey key] => ref Value[key];

        public bool ContainsKey(TKey key)                         => Value.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValueResult value) => Value.TryGetValue(key, out value);

        public Dictionary<TKey, TValueResult>           ToDictionary()  => Value.ToDictionary();
        public IEnumerator<ICovKVP<TKey, TValueResult>> GetEnumerator() => Value.GetEnumerator();
        IEnumerator IEnumerable.                        GetEnumerator() => GetEnumerator();

        #endregion
    }
}