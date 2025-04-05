using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

namespace NGDtuanh.BubleAsset {
    public struct BubleArray<T> :
        IBlobArrayWrapper<T>
      , IBlobBuildable<IReadOnlyCollection<T>>
      , IBlobBuildableSelf<BubleArray<T>>
        where T : struct {
        public BlobArray<T> Value;

        public void BuildBlob(ref BlobBuilder builder, IReadOnlyCollection<T> source)
            => builder.SetArrayDirectly(ref Value, source);

        public void BuildBlob(ref BlobBuilder builder, ref BubleArray<T> source)
            => builder.SetArrayDirectly(ref Value, ref source.Value);

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