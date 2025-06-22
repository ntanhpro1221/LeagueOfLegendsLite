using UnityEngine;

namespace NGDtuanh.Utils {
    public static class ObjectExtensions {
        public static string GetPath(this Transform trans) {
            if (trans == null) return string.Empty;
            string result = trans.name;
            while ((trans = trans.parent) != null)
                result = $"{trans.name}/{result}";
            return result;
        }

        public static string GetPath(this GameObject obj) =>
            GetPath(obj?.transform);

        public static void FlipActiveSelf(this GameObject obj) => obj.SetActive(!obj.activeSelf);
    }
}