using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EffectIconUI : MonoBehaviour {
    private readonly Dictionary<EffectFullId, EffectIconItemUI> _Items          = new();
    private readonly Stack<EffectIconItemUI>                    _AvailableItems = new();
    private readonly HashSet<EffectFullId>                      _TmpFixingItems = new();

    [SerializeField] private GameObject _ItemPrefab;
    [SerializeField] private Transform  _ItemRoot;

    private void Spawn(in EffectFullId id, EffectDataManaged.IconData data) {
        if (!_Items.ContainsKey(id))
            _Items.Add(id, _AvailableItems.Count > 0
                ? _AvailableItems.Pop()
                : Instantiate(_ItemPrefab, _ItemRoot).GetComponent<EffectIconItemUI>());

        _Items[id].InitUI(data);
        _Items[id].gameObject.SetActive(true);
    }

    private void Despawn(in EffectFullId id) {
        _Items[id].gameObject.SetActive(false);
        _AvailableItems.Push(_Items[id]);
        _Items.Remove(id);
    }

    public void FixAllUI(in DynamicBuffer<EffectBuffer> effectBuffer) {
        var effectRaws = GameSO.Effect;

        // Store all old key
        _TmpFixingItems.Clear();
        foreach (var id in _Items.Keys) _TmpFixingItems.Add(id);

        foreach (var effect in effectBuffer)
            // Remove key that has already existed
            if (_TmpFixingItems.Contains(effect.id))
                _TmpFixingItems.Remove(effect.id);
            // Spawn key that has NOT existed yet
            else if (effectRaws[effect.id.id].iconData.enable)
                Spawn(effect.id, effectRaws[effect.id.id].iconData);

        // Now we have list of old key that not exist in new effect buffer, just remove them 
        foreach (var id in _TmpFixingItems) Despawn(id);
    }
}