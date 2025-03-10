using System;
using System.Collections.Generic;
using System.Linq;

namespace BlobAssetExtend {
    public static class EquatableEnumExtensions {
        public static ICollection<TEnum> ToPureEnumCollection<TEnum>(this ICollection<EquatableEnum<TEnum>> source)
            where TEnum : unmanaged, Enum
            => source.Select(item => (TEnum)item).ToList();

        public static ICollection<EquatableEnum<TEnum>> ToEquatableEnumCollection<TEnum>(this ICollection<TEnum> source)
            where TEnum : unmanaged, Enum
            => source.Select(item => (EquatableEnum<TEnum>)item).ToList();
        
        public static ICollection<KeyValuePair<EquatableEnum<TEnumKey>, TValue>> ToEquatableEnumCollectionKey<TEnumKey, TValue>(this ICollection<KeyValuePair<TEnumKey, TValue>> source)
            where TEnumKey : unmanaged, Enum
            => source.Select(item => new KeyValuePair<EquatableEnum<TEnumKey>, TValue>(item.Key, item.Value)).ToList();
    }
}