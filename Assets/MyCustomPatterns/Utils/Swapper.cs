namespace NGDtuanh.Utils {
    public static class Swapper {
        public static void Swap<T>(ref T left, ref T right)
            => (left, right) = (right, left);

    }
}