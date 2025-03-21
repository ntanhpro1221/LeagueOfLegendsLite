using System.Collections.Generic;

namespace NGDtuanh.Utils {
    public static class ListExtensions {
        public static T Front<T>(this List<T> list) => list[0];
        public static T Back<T>(this  List<T> list) => list[^1];

        public static void PushBack<T>(this  List<T> list, T value) => list.Add(value);
        public static void PushBack<T>(this  List<T> list)          => list.Add(default);
        public static void PushFront<T>(this List<T> list, T value) => list.Insert(0, value);
        public static void PushFront<T>(this List<T> list) => list.Insert(0, default);

        public static T PopBack<T>(this List<T> list) {
            if (list.Count == 0) return default;
            var value = list.Back();
            list.RemoveAt(list.Count - 1);
            return value;
        }

        public static T PopFront<T>(this List<T> list) {
            if (list.Count == 0) return default;
            var value = list.Front();
            list.RemoveAt(0);
            return value;
        }
        
        public static void Swap<T>(this List<T> list, int leftId, int rightId)
            => (list[leftId], list[rightId]) = (list[rightId], list[leftId]);
    }
}