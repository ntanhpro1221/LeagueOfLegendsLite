using System;

namespace NGDtuanh.Utils {
    public static class MathExtensions {
        public static T IfOnly<T>(this T value, bool condition) => condition ? value : default;

        public static int  SubToZero(this int  value, int  amount) => Math.Max(0, value - amount);
        public static long SubToZero(this long value, long amount) => Math.Max(0, value - amount);
    }
}