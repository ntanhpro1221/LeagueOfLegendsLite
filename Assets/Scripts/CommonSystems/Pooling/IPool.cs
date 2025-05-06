using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public abstract class IPool<TItem> where TItem : struct {
    private Dictionary<TItem, Stack<GameObject>> _Available;
    private Dictionary<GameObject, TItem>        _ItemTypeLookup;

    protected abstract GameObject GetPrefab(TItem itemId);
    
    protected IPool() {
        _Available      = new();
        _ItemTypeLookup = new();
    }

    private Stack<GameObject> GetAvailable(TItem itemId) {
        if (!_Available.ContainsKey(itemId)) _Available.Add(itemId, new());
        return _Available[itemId];
    }
    
    public GameObject Instantiate(TItem itemId) {
        var available = GetAvailable(itemId);
        
        if (available.Count == 0) {
            var newObj = Object.Instantiate(GetPrefab(itemId));
            available.Push(newObj);
            _ItemTypeLookup.Add(newObj, itemId);
        }
        var result = available.Pop();
        
        result.SetActive(true);
        
        return result;
    }

    public void Destroy(GameObject item) {
        if (!_ItemTypeLookup.TryGetValue(item, out var itemId))
            throw new PoolException("i have never seen this object before");

        GetAvailable(itemId).Push(item);
        item.SetActive(false);
    }

    private class PoolException : Exception {
        public PoolException(string message) : base("NGDtuanh pool: " + message) { }
    }
}