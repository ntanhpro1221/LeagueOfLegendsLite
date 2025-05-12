using System;
using System.Collections.Generic;
using NGDtuanh.UnsafePooling;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public abstract class ICachedData<TItem> : ICleanupComponentData, IDisposable {
    public const int LAZY_INIT_CAPACITY   = 70;
    public const int DEFAULT_MAX_TICK_GAP = 70;

    protected readonly SortedDictionary<NetworkTick, HashSet<int>> _TickRefs      = new(NetworkTickComparer.Default);
    protected readonly Dictionary<int, TickAndData>                _Datas         = new(LAZY_INIT_CAPACITY);
    protected readonly Stack<NetworkTick>                          _Tmp_TickStack = new(LAZY_INIT_CAPACITY);

    protected bool        _IsDisposed = false;
    private   NetworkTick _NewestTick = new(0);

    protected class TickAndData {
        public HashSet<NetworkTick> tickRefs;
        public TItem              data;
        
        public TickAndData WithData(TItem _data) {
            data = _data;
            return this;
        }

        public TickAndData WithTickRefs(HashSet<NetworkTick> _tickRefs) {
            tickRefs = _tickRefs;
            return this;
        }
    }

    private void PopCode(int code) {
        if (!_Datas.TryGetValue(code, out var item)) return;

        // Reset value
        HashSetPool<NetworkTick>.Release(item.tickRefs);
        item.tickRefs = null;
        OnCleanupItem(ref item.data);

        // Release ptr and remove
        ObjectPool<TickAndData>.Release(item);
        _Datas.Remove(code);
    }

    protected void PopTick(NetworkTick tick) {
        if (!_TickRefs.TryGetValue(tick, out var codeSet)) return;

        // Reset value
        foreach (var code in codeSet) {
            _Datas[code].tickRefs.Remove(tick);
            if (_Datas[code].tickRefs.Count == 0)
                PopCode(code);
        }

        // Release ptr and remove
        HashSetPool<int>.Release(codeSet);
        _TickRefs.Remove(tick);
    }

    protected virtual void OnCleanupItem(ref TItem item) { }

    public bool ContainsCode(int code) =>
        _Datas.ContainsKey(code);

    public bool ContainsTick(int code, NetworkTick tick) =>
        _TickRefs.ContainsKey(tick)
     && _TickRefs[tick].Contains(code);

    public ref TItem GetData(int code) =>
        ref _Datas[code].data;

    public void PushTick(int code, NetworkTick tick) {
        // Try pool new key
        if (!_TickRefs.ContainsKey(tick)) _TickRefs.Add(tick, HashSetPool<int>.Claim());

        // Set tick cnt and push
        _TickRefs[tick].Add(code);
        _Datas[code].tickRefs.Add(tick);

        // Update the newest tick
        if (tick.IsNewerThan(_NewestTick)) _NewestTick = tick;
    }

    public ref TItem PushData(int code, TItem data) {
        _Datas.Add(code, ObjectPool<TickAndData>.Claim()
            .WithData(data)
            .WithTickRefs(HashSetPool<NetworkTick>.Claim()));
        return ref GetData(code);
    }

    protected void TrimOldData() {
        foreach (var tick in _TickRefs.Keys)
            if (_NewestTick.TicksSince(tick) <= DEFAULT_MAX_TICK_GAP) break;
            else _Tmp_TickStack.Push(tick);

        while (_Tmp_TickStack.Count > 0) PopTick(_Tmp_TickStack.Pop());
    }

    public void Dispose() {
        if (_IsDisposed) {
            Debug.LogWarning("NGDtuanh: You are calling dispose twice (if you are host, this is normal)");
            return;
        }
        _IsDisposed = true;
        
        DisposeAll();
    } 

    public virtual void DisposeAll() {
        // Just need to pop all tick
        foreach (var tick in _TickRefs.Keys) _Tmp_TickStack.Push(tick);

        while (_Tmp_TickStack.Count > 0) PopTick(_Tmp_TickStack.Pop());
    }
}