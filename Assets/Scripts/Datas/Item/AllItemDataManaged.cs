using System;
using NGDtuanh.Collections.EnumMap;
using Unity.Entities;

[Serializable]
public class AllItemDataManaged : EnumMap<ItemId, ItemDataManaged>, IComponentData { }