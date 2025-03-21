using System;
using NGDtuanh.Collections;
using Unity.Entities;

[Serializable]
public class AllItemDataManaged : CovEnumMap<ItemId, ItemDataManaged>, IComponentData { }