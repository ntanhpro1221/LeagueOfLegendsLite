using System;
using NGDtuanh.Collections;
using Unity.Collections;
using Unity.Entities;

/// <summary>
/// Only valid if your system run after <see cref="UpdateEffectMapSystem"/>.<br/>
/// </summary>
public struct EffectMap : ICleanupComponentData, IDisposable {
    public const int INIT_CAPACITY = 5;

    /// <summary>
    /// Get first (maybe last depend on <see cref="Update"/> function) index of effect that have corresponded id.
    /// </summary>
    private NativeHashMap<EqualEnum<EffectId>, int> data;

    public static EffectMap Construct() => new() { data = new NativeHashMap<EqualEnum<EffectId>, int>(INIT_CAPACITY, Allocator.Persistent) };

    public void Update(in DynamicBuffer<EffectBuffer> source) {
        data.Clear();
        for (int i = 0; i < source.Length; ++i)
            data.TryAdd(source[i].id.id, i);
    }

    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
    public readonly bool ContainsKey(EffectId id) => data.ContainsKey(id);

    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
    public readonly bool TryGetValue(EffectId id, out int value) => data.TryGetValue(id, out value);

    public int this[EffectId id] => data[id];

    public void Dispose() => data.Dispose();
}