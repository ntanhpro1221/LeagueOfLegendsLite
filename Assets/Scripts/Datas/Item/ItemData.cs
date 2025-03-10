using System;
using BlobAssetExtend;
using Unity.Entities;

[Serializable]
public struct ItemData : IConstructableFromOtherVersion<ItemDataManaged> {
    public ItemId                  id;
    public BlobArray<StatBuffData> buffs;

    public void Construct(BlobBuilder builder, IBaker baker, in ItemDataManaged dataManaged) {
        id = dataManaged.id;
        builder.SetArray(ref buffs, dataManaged.buffs);
    }
}