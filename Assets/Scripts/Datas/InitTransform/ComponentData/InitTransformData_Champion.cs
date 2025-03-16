using System.Collections.Generic;
using System.Linq;
using BlobAssetExtend;
using Unity.Entities;
using UnityEngine;

public struct InitTransformData_Champion : IConstructableFromOtherVersion<List<Transform>> {
    public BlobArray<InitTransformData> value;

    public void Construct(BlobBuilder builder, IBaker baker, in List<Transform> dataManaged) {
        builder.SetArray(ref value, dataManaged.Select(item => (InitTransformData)item).ToList());
    }
}