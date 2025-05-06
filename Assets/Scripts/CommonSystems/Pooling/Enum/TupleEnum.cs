using System;
using Unity.Collections.LowLevel.Unsafe;

public struct TupleEnum<T0, T1> : IEquatable<TupleEnum<T0, T1>>
    where T0 : struct, Enum
    where T1 : struct, Enum {
    public T0 Item0;
    public T1 Item1;
    
    public TupleEnum(T0 item0, T1 item1) => (Item0, Item1) = (item0, item1);

    public bool Equals(TupleEnum<T0, T1> other) =>
        UnsafeUtility.EnumEquals(Item0, other.Item0)
     && UnsafeUtility.EnumEquals(Item1, other.Item1);


    public override int GetHashCode() => HashCode.Combine(
        Item0
      , Item1);
}

// TupleEnum for 3 enums
public struct TupleEnum<T0, T1, T2> : IEquatable<TupleEnum<T0, T1, T2>>
    where T0 : struct, Enum
    where T1 : struct, Enum
    where T2 : struct, Enum {
    public T0 Item0;
    public T1 Item1;
    public T2 Item2;

    public TupleEnum(T0 item0, T1 item1, T2 item2)
        => (Item0, Item1, Item2) = (item0, item1, item2);

    public bool Equals(TupleEnum<T0, T1, T2> other)
        => UnsafeUtility.EnumEquals(Item0, other.Item0)
        && UnsafeUtility.EnumEquals(Item1, other.Item1)
        && UnsafeUtility.EnumEquals(Item2, other.Item2);

    public override bool Equals(object obj)
        => obj is TupleEnum<T0, T1, T2> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Item0, Item1, Item2);
}

// TupleEnum for 4 enums
public struct TupleEnum<T0, T1, T2, T3> : IEquatable<TupleEnum<T0, T1, T2, T3>>
    where T0 : struct, Enum
    where T1 : struct, Enum
    where T2 : struct, Enum
    where T3 : struct, Enum {
    public T0 Item0;
    public T1 Item1;
    public T2 Item2;
    public T3 Item3;

    public TupleEnum(T0 item0, T1 item1, T2 item2, T3 item3)
        => (Item0, Item1, Item2, Item3) = (item0, item1, item2, item3);

    public bool Equals(TupleEnum<T0, T1, T2, T3> other)
        => UnsafeUtility.EnumEquals(Item0, other.Item0)
        && UnsafeUtility.EnumEquals(Item1, other.Item1)
        && UnsafeUtility.EnumEquals(Item2, other.Item2)
        && UnsafeUtility.EnumEquals(Item3, other.Item3);

    public override bool Equals(object obj)
        => obj is TupleEnum<T0, T1, T2, T3> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Item0, Item1, Item2, Item3);
}

// TupleEnum for 5 enums
public struct TupleEnum<T0, T1, T2, T3, T4> : IEquatable<TupleEnum<T0, T1, T2, T3, T4>>
    where T0 : struct, Enum
    where T1 : struct, Enum
    where T2 : struct, Enum
    where T3 : struct, Enum
    where T4 : struct, Enum {
    public T0 Item0;
    public T1 Item1;
    public T2 Item2;
    public T3 Item3;
    public T4 Item4;

    public TupleEnum(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4)
        => (Item0, Item1, Item2, Item3, Item4) = (item0, item1, item2, item3, item4);

    public bool Equals(TupleEnum<T0, T1, T2, T3, T4> other)
        => UnsafeUtility.EnumEquals(Item0, other.Item0)
        && UnsafeUtility.EnumEquals(Item1, other.Item1)
        && UnsafeUtility.EnumEquals(Item2, other.Item2)
        && UnsafeUtility.EnumEquals(Item3, other.Item3)
        && UnsafeUtility.EnumEquals(Item4, other.Item4);

    public override bool Equals(object obj)
        => obj is TupleEnum<T0, T1, T2, T3, T4> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Item0, Item1, Item2, Item3, Item4);
}

// TupleEnum for 6 enums
public struct TupleEnum<T0, T1, T2, T3, T4, T5> : IEquatable<TupleEnum<T0, T1, T2, T3, T4, T5>>
    where T0 : struct, Enum
    where T1 : struct, Enum
    where T2 : struct, Enum
    where T3 : struct, Enum
    where T4 : struct, Enum
    where T5 : struct, Enum {
    public T0 Item0;
    public T1 Item1;
    public T2 Item2;
    public T3 Item3;
    public T4 Item4;
    public T5 Item5;

    public TupleEnum(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        => (Item0, Item1, Item2, Item3, Item4, Item5) = (item0, item1, item2, item3, item4, item5);

    public bool Equals(TupleEnum<T0, T1, T2, T3, T4, T5> other)
        => UnsafeUtility.EnumEquals(Item0, other.Item0)
        && UnsafeUtility.EnumEquals(Item1, other.Item1)
        && UnsafeUtility.EnumEquals(Item2, other.Item2)
        && UnsafeUtility.EnumEquals(Item3, other.Item3)
        && UnsafeUtility.EnumEquals(Item4, other.Item4)
        && UnsafeUtility.EnumEquals(Item5, other.Item5);

    public override bool Equals(object obj)
        => obj is TupleEnum<T0, T1, T2, T3, T4, T5> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Item0, Item1, Item2, Item3, Item4, Item5);
}

// TupleEnum for 7 enums
public struct TupleEnum<T0, T1, T2, T3, T4, T5, T6> : IEquatable<TupleEnum<T0, T1, T2, T3, T4, T5, T6>>
    where T0 : struct, Enum
    where T1 : struct, Enum
    where T2 : struct, Enum
    where T3 : struct, Enum
    where T4 : struct, Enum
    where T5 : struct, Enum
    where T6 : struct, Enum {
    public T0 Item0;
    public T1 Item1;
    public T2 Item2;
    public T3 Item3;
    public T4 Item4;
    public T5 Item5;
    public T6 Item6;

    public TupleEnum(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        => (Item0, Item1, Item2, Item3, Item4, Item5, Item6) = (item0, item1, item2, item3, item4, item5, item6);

    public bool Equals(TupleEnum<T0, T1, T2, T3, T4, T5, T6> other)
        => UnsafeUtility.EnumEquals(Item0, other.Item0)
        && UnsafeUtility.EnumEquals(Item1, other.Item1)
        && UnsafeUtility.EnumEquals(Item2, other.Item2)
        && UnsafeUtility.EnumEquals(Item3, other.Item3)
        && UnsafeUtility.EnumEquals(Item4, other.Item4)
        && UnsafeUtility.EnumEquals(Item5, other.Item5)
        && UnsafeUtility.EnumEquals(Item6, other.Item6);

    public override bool Equals(object obj)
        => obj is TupleEnum<T0, T1, T2, T3, T4, T5, T6> other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Item0, Item1, Item2, Item3, Item4, Item5, Item6);
}
