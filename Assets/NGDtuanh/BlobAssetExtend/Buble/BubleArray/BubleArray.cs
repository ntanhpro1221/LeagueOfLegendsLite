using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    public struct BubleArray<TResult, TSource> :
        IBlobArrayWrapper<TResult>
      , IBlobBuildable<IReadOnlyCollection<TSource>>
        where TResult : struct, IBlobBuildable<TSource> {
        public BlobArray<TResult> Value;

        public void BuildBlob(ref BlobBuilder builder, IReadOnlyCollection<TSource> source, IBaker baker)
            => builder.SetArray(ref Value, source, baker);

        #region BLOB ARRAY FUNCTIONS

        public        int   Count          => Value.Length;
        public unsafe void* GetUnsafePtr() => Value.GetUnsafePtr();
        public ref TResult this[int index] => ref Value[index];
        public TResult[] ToArray() => Value.ToArray();

        public IEnumerator<TResult> GetEnumerator() {
            for (int i = 0; i < Count; i++)
                yield return Value[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}