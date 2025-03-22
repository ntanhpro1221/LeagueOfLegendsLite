using System;
using System.Collections.Generic;
using System.Linq;

namespace NGDtuanh.Collections {
    public static class CovDictionaryExtensions {
        public static CovDictionary<TKey, TValue> ToCovariance<TKey, TValue>(
            this Dictionary<TKey, TValue> dict) => new(dict);

        public static CovDictionary<TEnumKey, TValue> CastKey_Enum<TEnumKey, TValue>(
            this IEnumerable<ICovKVP<EqualEnum<TEnumKey>, TValue>> source)
            where TEnumKey : struct, Enum
            => source.ToDictionary(item => (TEnumKey)item.Key, item => item.Value).ToCovariance();

        public static CovDictionary<EqualEnum<TEnumKey>, TValue> CastKey_EqualEnum<TEnumKey, TValue>(
            this IEnumerable<ICovKVP<TEnumKey, TValue>> source)
            where TEnumKey : struct, Enum
            => source.ToDictionary(item => (EqualEnum<TEnumKey>)item.Key, item => item.Value).ToCovariance();
    }
}