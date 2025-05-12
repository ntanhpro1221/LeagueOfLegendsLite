using System.Collections.Generic;
using Unity.NetCode;

public readonly struct NetworkTickComparer : IComparer<NetworkTick> {
    public static readonly NetworkTickComparer Default = default;

    public int Compare(NetworkTick lhs, NetworkTick rhs)
        => lhs.Equals(rhs) ? 0 : lhs.IsNewerThan(rhs) ? 1 : -1;
}