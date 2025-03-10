namespace BlobAssetExtend {
    public struct KeyIndex {
        public int first;
        public int count;

        public KeyIndex(int first, int count)
            => (this.first, this.count) = (first, count);

        public readonly int GetLast() => first + count - 1;
    }
}