using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace NGDtuanh.Collections {
    public struct EqualEnum<TEnum> :
        IEquatable<EqualEnum<TEnum>>
        where TEnum : struct, Enum {
        public TEnum Value;

        public EqualEnum(in TEnum            value) => Value = value;
        public EqualEnum(in EqualEnum<TEnum> value) => Value = value;

        public static implicit operator TEnum(EqualEnum<TEnum> value)
            => value.Value;

        public static implicit operator EqualEnum<TEnum>(TEnum value)
            => new(value);

        public bool Equals(EqualEnum<TEnum> other)
            => UnsafeUtility.EnumEquals(Value, other.Value);

        public override bool Equals(object obj)
            => obj is EqualEnum<TEnum> other && Equals(other);

        public override int GetHashCode() => UnsafeUtility.EnumToInt(Value);
    }
}