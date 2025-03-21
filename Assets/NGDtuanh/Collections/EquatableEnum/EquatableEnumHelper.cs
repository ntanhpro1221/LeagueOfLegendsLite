using System;
using System.Collections.Generic;
using System.Linq;

namespace NGDtuanh.Collections {
    public static class EquatableEnumHelper {
        public static List<TEnum> Cast_Enum<TEnum>(
            this IEnumerable<EquatableEnum<TEnum>> source)
            where TEnum : struct, Enum
            => source.Select(item => (TEnum)item).ToList();

        public static List<EquatableEnum<TEnum>> Cast_EqualEnum<TEnum>(
            this IEnumerable<TEnum> source)
            where TEnum : struct, Enum
            => source.Select(item => (EquatableEnum<TEnum>)item).ToList();

        public static Dictionary<TEnumKey, TValue> CastKey_Enum<TEnumKey, TValue>(
            this IEnumerable<KeyValuePair<EquatableEnum<TEnumKey>, TValue>> source)
            where TEnumKey : struct, Enum
            => source.ToDictionary(item => (TEnumKey)item.Key, item => item.Value);

        public static Dictionary<EquatableEnum<TEnumKey>, TValue> CastKey_EqualEnum<TEnumKey, TValue>(
            this IEnumerable<KeyValuePair<TEnumKey, TValue>> source)
            where TEnumKey : struct, Enum
            => source.ToDictionary(item => (EquatableEnum<TEnumKey>)item.Key, item => item.Value);
    }
}