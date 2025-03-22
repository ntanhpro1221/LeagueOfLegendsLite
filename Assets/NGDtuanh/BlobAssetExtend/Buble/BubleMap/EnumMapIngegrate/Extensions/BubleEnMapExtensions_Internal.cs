using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    public static class BubleEnMapExtensions_Internal {
        public static BlobBuilderMap<EqualEnum<TKey>, TValueResult> Allocate<TKey, TValueResult, TValueSource>(
            this ref BlobBuilder                                  builder
          , ref      BubleEnMap<TKey, TValueResult, TValueSource> ptr
          , IReadOnlyCollection<TKey>                             rawKeys)
            where TKey : struct, Enum
            where TValueResult : struct, IBlobBuildable<TValueSource>
            => builder.Allocate(ref ptr.Value, rawKeys.Cast_EqualEnum());
    }
}