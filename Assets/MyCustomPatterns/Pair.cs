using System;

[Serializable]
public struct Pair<T1, T2>
    where T1 : unmanaged
    where T2 : unmanaged {
    public T1 first;
    public T2 second;

    public Pair(in T1 first, in T2 second)
        => (this.first, this.second) = (first, second);

    [Serializable]
    public class Managed<T1Managed, T2Managed> {
        public T1 first;
        public T2 second;

        public Managed(in T1 first, in T2 second)
            => (this.first, this.second) = (first, second);
    }
}