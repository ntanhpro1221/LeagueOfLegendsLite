using Unity.Entities;

public static class BuffExtensions {
    public static void InsertSorted(
        this ref DynamicBuffer<IncomingExpiringBuffBuffer> buffer
      , in       IncomingExpiringBuffBuffer                item) {
        if (buffer.IsEmpty) {
            buffer.Add(item);
            return;
        }

        // Expire buffs are sorted from newest to oldest.
        // We will use binary search to find the smallest index l that buffer[l] is older than or equal item.
        int l = 0, r = buffer.Length - 1;
        while (l < r) {
            int m = (l + r) / 2;
            if (buffer[m].expireAtTick.IsNewerThan(item.expireAtTick))
                l  = m + 1;
            else r = m;
        }

        buffer.Insert(l, item);
    }
}