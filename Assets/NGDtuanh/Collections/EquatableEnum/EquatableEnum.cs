using System;
using Unity.Collections.LowLevel.Unsafe;

namespace NGDtuanh.Collections {
    public struct EquatableEnum<TEnum> : 
        IEquatable<EquatableEnum<TEnum>> 
        where TEnum : struct, Enum {
        private TEnum _Value;

        public EquatableEnum(in TEnum                value) => _Value = value;
        public EquatableEnum(in EquatableEnum<TEnum> value) => _Value = value;

        public static implicit operator TEnum(EquatableEnum<TEnum> value)
            => value._Value;

        public static implicit operator EquatableEnum<TEnum>(TEnum value)
            => new(value);

        public bool Equals(EquatableEnum<TEnum> other)
            => UnsafeUtility.EnumEquals(_Value, other._Value);

        public override bool Equals(object obj)
            => obj is EquatableEnum<TEnum> other && Equals(other);

        public override int GetHashCode() => UnsafeUtility.EnumToInt(_Value);
    }
}