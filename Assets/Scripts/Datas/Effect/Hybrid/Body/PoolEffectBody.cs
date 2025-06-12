using System.Collections.Generic;
using NGDtuanh.Collections;
using NGDtuanh.Singleton;
using UnityEngine;

public class PoolEffectBody : SceneSingleton<PoolEffectBody> {
    [SerializeField] private EnumMap<EffectBodyId, IEffectBodyItemUI> _Prefabs;

    private readonly Dictionary<EffectBodyId, Stack<IEffectBodyItemUI>> _Available = new();

    public void Release(EffectBodyId id, ref IEffectBodyItemUI item) {
        _Available[id].Push(item);
        item.Stop();
        item = null;
    }

    public IEffectBodyItemUI Get(EffectBodyId id, Transform root) {
        if (!_Available.ContainsKey(id)) _Available.Add(id, new());

        if (_Available[id].Count == 0)
            _Available[id].Push(Instantiate(_Prefabs[id].gameObject).GetComponent<IEffectBodyItemUI>());

        var result = _Available[id].Pop();
        result.Play();

        result.transform.SetParent(root);
        result.transform.localPosition = Vector3.zero;

        return result;
    }
}