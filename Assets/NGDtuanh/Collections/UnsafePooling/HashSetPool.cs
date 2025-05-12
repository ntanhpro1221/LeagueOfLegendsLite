using System.Collections.Generic;

namespace NGDtuanh.UnsafePooling {
    public class HashSetPool<TItem> {
        private static Stack<HashSet<TItem>> _Pool = new(PoolHelpers.LAZY_INIT_CAPACITY);

        private static HashSet<TItem> SpawnItem()
            => new();

        private static void CleanupItem(HashSet<TItem> item)
            => item.Clear();

        public static HashSet<TItem> Claim()
            => PoolHelpers.Claim(_Pool, SpawnItem);

        public static void Release(HashSet<TItem> item)
            => PoolHelpers.Release(_Pool, item, CleanupItem);
    }
}