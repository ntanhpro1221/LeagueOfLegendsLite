using System;
using System.Collections;
using System.Collections.Generic;
using NGDtuanh.Collections;
using NGDtuanh.Utils;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    public struct BubleMap<TKey, TValueResult, TValueSource> :
        IBlobMapWrapper<TKey, TValueResult>
      , IBlobBuildable<ICovKVPCollection<TKey, TValueSource>>
        where TKey : struct, IEquatable<TKey>
        where TValueResult : struct, IBlobBuildable<TValueSource> {
        public BlobMap<TKey, TValueResult> Value;

        public void BuildBlob(ref BlobBuilder builder, ICovKVPCollection<TKey, TValueSource> source, IBaker baker)
            => builder.SetMap(ref Value, source, baker);

        #region BLOB MAP FUCNTIONS

        public int Count => Value.Count;

        public ref TValueResult this[TKey key] => ref Value[key];

        public bool ContainsKey(TKey key)                         => Value.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValueResult value) => Value.TryGetValue(key, out value);

        public Dictionary<TKey, TValueResult>                   ToDictionary()  => Value.ToDictionary();
        public IEnumerator<KVPairUnmanaged<TKey, TValueResult>> GetEnumerator() => Value.GetEnumerator();
        IEnumerator IEnumerable.                                GetEnumerator() => GetEnumerator();

        #endregion
    }
}