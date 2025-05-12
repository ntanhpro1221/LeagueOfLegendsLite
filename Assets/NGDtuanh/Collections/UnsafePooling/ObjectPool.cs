using System.Collections.Generic;

namespace NGDtuanh.UnsafePooling {
    public class ObjectPool<TItem> where TItem : class, new() {
        private static Stack<TItem> _Pool = new(PoolHelpers.LAZY_INIT_CAPACITY);

        private static TItem SpawnItem() 
            => new();

        public static TItem Claim() 
            => PoolHelpers.Claim(_Pool, SpawnItem);

        public static void Release(TItem item, in PoolHelpers.CleanupItem<TItem> cleanupItemDel = null) 
            => PoolHelpers.Release(_Pool, item, cleanupItemDel);
    }
}