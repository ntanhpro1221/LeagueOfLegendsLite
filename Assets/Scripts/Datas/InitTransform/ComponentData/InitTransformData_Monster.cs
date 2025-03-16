using System.Collections.Generic;
using BlobAssetExtend;
using NGDtuanh.Collections.EnumMap;
using Unity.Entities;
using UnityEngine;

public struct InitTransformData_Monster : IConstructableFromOtherVersion<EnumMap<TransformKeys.Monster, Transform>> {
    public BlobHashMap<EquatableEnum<TransformKeys.Monster>, InitTransformData> value;

    public void Construct(BlobBuilder                                  builder
                        , IBaker                                       baker
                        , in EnumMap<TransformKeys.Monster, Transform> dataManaged) {
        Dictionary<EquatableEnum<TransformKeys.Monster>, InitTransformData> source = new();
        foreach (var (key, value) in dataManaged) source.Add(key, value);
        builder.SetHashMap(ref value, source);
    }
}