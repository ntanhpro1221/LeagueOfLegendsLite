using System;
using NGDtuanh.PropertySet;
using Unity.Entities;

[Serializable]
public class AllItemDataManaged : PropertySet<ItemId, ItemDataManaged>, IComponentData { }