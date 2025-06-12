using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct ItemData : IBlobBuildable<ItemDataManaged>, IBlobBuildableSelf<ItemData> {
    // public BubleArray<StatBuffs.Sender> statBuffs;

    public void BuildBlob(ref BlobBuilder builder, ItemDataManaged source) {
        // statBuffs.BuildBlob(ref builder, source.statBuffs);
    }

    public void BuildBlob(ref BlobBuilder builder, ref ItemData source) {
        // statBuffs.BuildBlob(ref builder, ref source.statBuffs);
    }
}