using System;
using System.Collections.Generic;
using System.Linq;

namespace NGDtuanh.Collections {
    public static class EquatableEnumHelper {
        public static List<TEnum> Cast_Enum<TEnum>(
            this IEnumerable<EqualEnum<TEnum>> source)
            where TEnum : struct, Enum
            => source.Select(item => (TEnum)item).ToList();

        public static List<EqualEnum<TEnum>> Cast_EqualEnum<TEnum>(
            this IEnumerable<TEnum> source)
            where TEnum : struct, Enum
            => source.Select(item => (EqualEnum<TEnum>)item).ToList();

        public static Dictionary<TEnumKey, TValue> CastKey_Enum<TEnumKey, TValue>(
            this IEnumerable<KeyValuePair<EqualEnum<TEnumKey>, TValue>> source)
            where TEnumKey : struct, Enum
            => source.ToDictionary(item => (TEnumKey)item.Key, item => item.Value);

        public static Dictionary<EqualEnum<TEnumKey>, TValue> CastKey_EqualEnum<TEnumKey, TValue>(
            this IEnumerable<KeyValuePair<TEnumKey, TValue>> source)
            where TEnumKey : struct, Enum
            => source.ToDictionary(item => (EqualEnum<TEnumKey>)item.Key, item => item.Value);
    }
}