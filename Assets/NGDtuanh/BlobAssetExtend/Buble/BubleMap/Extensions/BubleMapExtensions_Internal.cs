using System;
using System.Collections.Generic;
using Unity.Entities;

namespace NGDtuanh.BlobAssetExtend {
    internal static class BubleMapExtensions_Internal {
        public static BlobBuilderMap<TKey, TValueResult> Allocate<TKey, TValueResult, TValueSource>(
            this ref BlobBuilder                                 builder
          , ref      BubleMap<TKey, TValueResult, TValueSource> ptr
          , IReadOnlyCollection<TKey>                            rawKeys)
            where TKey : struct, IEquatable<TKey>
            where TValueResult : struct, IBlobBuildable<TValueSource>
            => builder.Allocate(ref ptr.Value, rawKeys);
    }
}