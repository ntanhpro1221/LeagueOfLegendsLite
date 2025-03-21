using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    public struct BubleArray<T> :
        IBlobArrayWrapper<T>
      , IBlobBuildable<IReadOnlyCollection<T>>
        where T : struct {
        public BlobArray<T> Value;

        public void BuildBlob(ref BlobBuilder builder, IReadOnlyCollection<T> source, IBaker baker)
            => builder.SetArray(ref Value, source, baker);

        #region BLOB ARRAY FUNCTIONS

        public        int   Count          => Value.Length;
        public unsafe void* GetUnsafePtr() => Value.GetUnsafePtr();
        public ref T this[int index] => ref Value[index];
        public T[] ToArray() => Value.ToArray();

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < Count; i++)
                yield return Value[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}