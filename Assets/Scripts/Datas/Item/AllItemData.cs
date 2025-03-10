using System;
using BlobAssetExtend;
using Unity.Entities;

[Serializable]
public struct AllItemData : IComponentData {
    public BlobAssetReference<BlobHashMap<EquatableEnum<ItemId>, ItemData>> items;
}