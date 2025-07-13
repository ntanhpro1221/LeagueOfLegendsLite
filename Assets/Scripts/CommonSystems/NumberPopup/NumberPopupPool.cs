using System.Collections.Generic;
using NGDtuanh.Singleton;
using UnityEngine;

public class NumberPopupPool : SceneSingleton<NumberPopupPool> {
    private Transform _Trans;

    private readonly Stack<NumberPopup> _AvailableItem = new();

    [SerializeField] private NumberPopup _ItemPrefab;

    protected override void OnTouched() {
        base.OnTouched();

        _Trans = transform;
    }

    public NumberPopup GetItem(NumberPopup.Id id) {
        if (_AvailableItem.Count == 0) _AvailableItem.Push(Instantiate(_ItemPrefab, _Trans));

        var result = _AvailableItem.Pop();
        result.gameObject.SetActive(true);
        return result;
    }

    public void ReleaseItem(NumberPopup item) {
        item.gameObject.SetActive(false);
        item.Trans.SetParent(_Trans);
        _AvailableItem.Push(item);
    }
}