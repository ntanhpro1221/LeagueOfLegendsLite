using System.Collections.Generic;

namespace NGDtuanh.BlobAssetExtend {
    internal interface IBlobArrayWrapper<T> :
        IReadOnlyCollection<T>
        where T : struct {
        unsafe void* GetUnsafePtr();
        ref T this[int index] { get; }
        T[] ToArray();
    }
}