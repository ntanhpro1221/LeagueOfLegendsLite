using System;
using NGDtuanh.BlobAssetExtend;
using Unity.Entities;

[Serializable]
public struct ItemData : IBlobBuildable<ItemDataManaged> {
    public ItemId        id;
    public BubleArray<StatBuffData> buffs;

    public void BuildBlob(ref BlobBuilder builder, ItemDataManaged source, IBaker baker) {
        id = source.id;
        buffs.BuildBlob(ref builder, source.buffs, baker);
    }
}