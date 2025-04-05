using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct AllItemData : IComponentData {
    public BlobAssetReference<BubleEnMap<ItemId, ItemData, ItemDataManaged>> _Ref; 
    
    public ref BubleEnMap<ItemId, ItemData, ItemDataManaged> Items => ref _Ref.Value;
}