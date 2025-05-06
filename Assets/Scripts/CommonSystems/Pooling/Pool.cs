using System;
using NGDtuanh.Collections;
using UnityEngine;

[Serializable]
public class Pool<TItem> : IPool<TItem> where TItem : struct, Enum {
    [SerializeField] private EnumMap<TItem, GameObject> _Prefabs;
    
    public Pool() : base() { }
    
    protected override GameObject GetPrefab(TItem itemId) => _Prefabs[itemId];
}

[Serializable]
public class Pool<TItem0, TItem1> : IPool<TupleEnum<TItem0, TItem1>>
    where TItem0 : struct, Enum
    where TItem1 : struct, Enum {
    [SerializeField] private EnumMap<TItem0, EnumMap<TItem1, GameObject>> _Prefabs;
    
    public Pool() : base() { }

    protected override GameObject GetPrefab(TupleEnum<TItem0, TItem1> itemId) =>
        _Prefabs[itemId.Item0][itemId.Item1];
}
