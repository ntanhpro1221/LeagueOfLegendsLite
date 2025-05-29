using System;
using Unity.Burst;

namespace NGDtuanh.BubleAsset {
    [BurstCompile]
    public static class BlobMapHasher<TKey> {
        [BurstCompile]
        public static int GetHashedKey(TKey key, int count) {
            return (int)(((ulong)key.GetHashCode() * 11400714819323198485ul) >> 33) % count;
        }
    }
}