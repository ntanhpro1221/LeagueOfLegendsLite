using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct ItemData : IBlobBuildable<ItemDataManaged>, IBlobBuildableSelf<ItemData> {
    public BubleArray<IncomingBuffBuffer> buffs;

    public void BuildBlob(ref BlobBuilder builder, ItemDataManaged source) {
        buffs.BuildBlob(ref builder, source.buffs);
    }

    public void BuildBlob(ref BlobBuilder builder, ref ItemData source) {
        buffs.BuildBlob(ref builder, ref source.buffs);
    }
}