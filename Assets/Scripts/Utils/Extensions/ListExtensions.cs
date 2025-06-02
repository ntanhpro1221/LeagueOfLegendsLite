using System.Collections.Generic;

public static class ListExtensions {
    public static void FixToSize<T>(this List<T> list, int size) {
        while (list.Count > size) list.RemoveAt(list.Count - 1);
        while (list.Count < size) list.Add(default);
    }
}