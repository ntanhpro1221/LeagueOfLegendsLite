using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class EffectBarUI : MonoBehaviour {
    private readonly Dictionary<EffectFullId, EffectBarItemUI> _Items          = new();
    private readonly Stack<EffectBarItemUI>                    _AvailableItems = new();
    private readonly HashSet<EffectFullId>                     _TmpFixingItems = new();

    [SerializeField] private GameObject _ItemPrefab;
    [SerializeField] private Transform  _ItemRoot;

    private void Spawn(in EffectFullId id, EffectDataManaged data, in FixedString64Bytes sourceName) {
        if (!_Items.ContainsKey(id))
            _Items.Add(id, _AvailableItems.Count > 0
                ? _AvailableItems.Pop()
                : Instantiate(_ItemPrefab, _ItemRoot).GetComponent<EffectBarItemUI>());

        _Items[id].InitUI(data, sourceName);
        _Items[id].gameObject.SetActive(true);
    }

    private void Despawn(in EffectFullId id) {
        _Items[id].gameObject.SetActive(false);
        _AvailableItems.Push(_Items[id]);
        _Items.Remove(id);
    }

    public void UpdateAllUI(in NetworkTick curTick, in DynamicBuffer<EffectBuffer> effectBuffer) {
        foreach (var effect in effectBuffer)
            if (_Items.TryGetValue(effect.id, out var item))
                item.UpdateUI(curTick, effect);
    }

    public void FixAllUI(in DynamicBuffer<EffectBuffer> effectBuffer, in ComponentLookup<SetNameRequest> nameLookup) {
        var effectRaws = GameSO.Effect;

        // Store all old key
        _TmpFixingItems.Clear();
        foreach (var id in _Items.Keys) _TmpFixingItems.Add(id);

        foreach (var effect in effectBuffer)
            // Remove key that has already existed
            if (_TmpFixingItems.Contains(effect.id))
                _TmpFixingItems.Remove(effect.id);
            // Spawn key that has NOT existed yet
            else if (effectRaws[effect.id.id].barData.enable)
                Spawn(effect.id, effectRaws[effect.id.id], nameLookup[effect.id.source].name);

        // Now we have list of old key that not exist in new effect buffer, just remove them 
        foreach (var id in _TmpFixingItems) Despawn(id);
    }
}