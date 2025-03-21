namespace NGDtuanh.Collections {
    public interface ICovKVP<out TKey, out TValue> {
        TKey   Key   { get; }
        TValue Value { get; }
    }
}