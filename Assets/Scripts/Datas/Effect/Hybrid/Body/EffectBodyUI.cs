using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EffectBodyUI : MonoBehaviour {
    private class BodyItem {
        public int               count;
        public IEffectBodyItemUI body;
    }

    private readonly Dictionary<EffectFullId, EffectBodyId> _Items          = new();
    private readonly Dictionary<EffectBodyId, BodyItem>     _Bodies         = new();
    private readonly HashSet<EffectFullId>                  _TmpFixingItems = new();

    [SerializeField] private Transform _ItemRoot;

    private void Spawn(in EffectFullId id, EffectDataManaged.BodyData data) {
        var bodyId = data.bodyId;

        if (!_Items.ContainsKey(id)) _Items.Add(id, default);
        _Items[id] = bodyId;

        if (!_Bodies.ContainsKey(bodyId)) _Bodies.Add(bodyId, new());
        if (_Bodies[bodyId].body == null) _Bodies[bodyId].body = PoolEffectBody.Instance.Get(bodyId, _ItemRoot);
        ++_Bodies[bodyId].count;
    }
  
    private void Despawn(in EffectFullId id) {
        var bodyId = _Items[id];
        _Items.Remove(id);

        if (--_Bodies[bodyId].count == 0)
            PoolEffectBody.Instance.Release(bodyId, ref _Bodies[bodyId].body);
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
            else if (effectRaws[effect.id.id].bodyData.enable)
                Spawn(effect.id, effectRaws[effect.id.id].bodyData);

        // Now we have list of old key that not exist in new effect buffer, just remove them 
        foreach (var id in _TmpFixingItems) Despawn(id);
    }
}