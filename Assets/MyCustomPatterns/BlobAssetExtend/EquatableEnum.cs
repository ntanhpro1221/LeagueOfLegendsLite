using System;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace BlobAssetExtend {
    public struct EquatableEnum<TEnum> : IEquatable<EquatableEnum<TEnum>> where TEnum : unmanaged, Enum {
        private TEnum _Value;

        public EquatableEnum(TEnum                value) => _Value = value;
        public EquatableEnum(EquatableEnum<TEnum> value) => _Value = value;

        public static implicit operator TEnum(EquatableEnum<TEnum> value)
            => value._Value;

        public static implicit operator EquatableEnum<TEnum>(TEnum value)
            => new() { _Value = value };

        public bool Equals(EquatableEnum<TEnum> other)
            => UnsafeUtility.EnumEquals(_Value, other._Value);

        public override bool Equals(object obj)
            => obj is EquatableEnum<TEnum> other && Equals(other);

        public override int GetHashCode()
            => _Value.GetHashCode();
    }
}