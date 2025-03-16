using System.Collections.Generic;
using BlobAssetExtend;
using NGDtuanh.Collections.EnumMap;
using Unity.Entities;
using UnityEngine;

public struct InitTransformData_BigMonster : IConstructableFromOtherVersion<EnumMap<TransformKeys.BigMonster, Transform>> {
    public BlobHashMap<EquatableEnum<TransformKeys.BigMonster>, InitTransformData> value;

    public void Construct(BlobBuilder                                     builder
                        , IBaker                                          baker
                        , in EnumMap<TransformKeys.BigMonster, Transform> dataManaged) {
        Dictionary<EquatableEnum<TransformKeys.BigMonster>, InitTransformData> source = new();
        foreach (var (key, value) in dataManaged) source.Add(key, value);
        builder.SetHashMap(ref value, source);
    }
}