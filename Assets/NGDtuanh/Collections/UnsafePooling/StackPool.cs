using System.Collections.Generic;

namespace NGDtuanh.UnsafePooling {
    public class StackPool<TItem> {
        private static Stack<Stack<TItem>> _Pool = new(PoolHelpers.LAZY_INIT_CAPACITY);

        private static Stack<TItem> SpawnItem() 
            => new();

        private static void CleanupItem(Stack<TItem> item) 
            => item.Clear();

        public static Stack<TItem> Claim() 
            => PoolHelpers.Claim(_Pool, SpawnItem);

        public static void Release(Stack<TItem> item) 
            => PoolHelpers.Release(_Pool, item, CleanupItem);
    }
}