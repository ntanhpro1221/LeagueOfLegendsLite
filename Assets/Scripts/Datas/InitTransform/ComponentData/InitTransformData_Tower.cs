using System.Collections.Generic;
using BlobAssetExtend;
using NGDtuanh.Collections.EnumMap;
using Unity.Entities;
using UnityEngine;

public struct InitTransformData_Tower : IConstructableFromOtherVersion<EnumMap<TransformKeys.Tower, Transform>> {
    public BlobHashMap<EquatableEnum<TransformKeys.Tower>, InitTransformData> value;

    public void Construct(BlobBuilder                                builder
                        , IBaker                                     baker
                        , in EnumMap<TransformKeys.Tower, Transform> dataManaged) {
        Dictionary<EquatableEnum<TransformKeys.Tower>, InitTransformData> source = new();
        foreach (var (key, value) in dataManaged) source.Add(key, value);
        builder.SetHashMap(ref value, source);
    }
}