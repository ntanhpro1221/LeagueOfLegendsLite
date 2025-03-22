using System;
using Unity.Collections.LowLevel.Unsafe;

namespace NGDtuanh.Collections {
    public struct EqualEnum<TEnum> : 
        IEquatable<EqualEnum<TEnum>> 
        where TEnum : struct, Enum {
        private TEnum _Value;

        public EqualEnum(in TEnum                value) => _Value = value;
        public EqualEnum(in EqualEnum<TEnum> value) => _Value = value;

        public static implicit operator TEnum(EqualEnum<TEnum> value)
            => value._Value;

        public static implicit operator EqualEnum<TEnum>(TEnum value)
            => new(value);

        public bool Equals(EqualEnum<TEnum> other)
            => UnsafeUtility.EnumEquals(_Value, other._Value);

        public override bool Equals(object obj)
            => obj is EqualEnum<TEnum> other && Equals(other);

        public override int GetHashCode() => UnsafeUtility.EnumToInt(_Value);
    }
}