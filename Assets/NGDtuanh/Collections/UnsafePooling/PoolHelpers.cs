using System.Collections.Generic;

namespace NGDtuanh.UnsafePooling {
    public static class PoolHelpers {
        public const int LAZY_INIT_CAPACITY = 5;

        public delegate TItem SpawnItemDel<out TItem>();

        public delegate void CleanupItem<in TItem>(TItem item);

        public static TItem Claim<TItem>(in Stack<TItem> pool, in SpawnItemDel<TItem> spawnItemDel) {
            if (pool.Count == 0) pool.Push(spawnItemDel());
            return pool.Pop();
        }

        public static void Release<TItem>(in Stack<TItem> pool, TItem item, in CleanupItem<TItem> cleanupItemDel = null) {
            cleanupItemDel?.Invoke(item);
            pool.Push(item);
        }
    }
}