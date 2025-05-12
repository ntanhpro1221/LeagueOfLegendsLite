using System.Collections.Generic;

namespace NGDtuanh.UnsafePooling {
    public class ListPool<TItem> {
        private static Stack<List<TItem>> _Pool = new(PoolHelpers.LAZY_INIT_CAPACITY);

        private static List<TItem> SpawnItem() 
            => new();

        private static void CleanupItem(List<TItem> item) 
            => item.Clear();

        public static List<TItem> Claim() 
            => PoolHelpers.Claim(_Pool, SpawnItem);

        public static void Release(List<TItem> item) 
            => PoolHelpers.Release(_Pool, item, CleanupItem);
    }
}