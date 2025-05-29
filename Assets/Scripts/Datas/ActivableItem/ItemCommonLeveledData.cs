using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct ItemCommonLeveledData : IBlobBuildableSelf<ItemCommonLeveledData> {
    public float_Q3           cooldownTime;
    public ItemActivationCost cost;

    public void BuildBlob(ref BlobBuilder builder, ref ItemCommonLeveledData source) {
        cooldownTime = source.cooldownTime;
        cost         = source.cost;
    }
}