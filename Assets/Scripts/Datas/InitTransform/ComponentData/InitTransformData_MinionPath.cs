using System.Collections.Generic;
using System.Linq;
using BlobAssetExtend;
using NGDtuanh.Collections.EnumMap;
using Unity.Entities;
using UnityEngine;

public struct InitTransformData_MinionPath : IConstructableFromOtherVersion<EnumMap<TransformKeys.MinionPath, List<Transform>>> {
    public BlobHashMap<EquatableEnum<TransformKeys.MinionPath>, BlobArray<InitTransformData>> value;

    public void Construct(BlobBuilder                                           builder
                        , IBaker                                                baker
                        , in EnumMap<TransformKeys.MinionPath, List<Transform>> dataManaged) {
        var keys         = dataManaged.Keys.ToEquatableEnumCollection();
        var transBuilder = builder.Allocate(ref value, dataManaged.Keys.ToEquatableEnumCollection());
        foreach (var key in keys)
            builder.SetArray(
                ref transBuilder[key]
              , dataManaged[key].Select(item => (InitTransformData)item).ToList());
        
    }
}