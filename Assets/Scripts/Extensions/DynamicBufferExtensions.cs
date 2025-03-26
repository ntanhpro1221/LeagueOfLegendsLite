using System;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public static class DynamicBufferExtensions {
    [BurstCompile]
    public static bool Contains<TBuffer, TValue>(
        this DynamicBuffer<TBuffer> buffer
      , TValue                      value)
        where TBuffer : unmanaged, IEquatable<TValue> {
        foreach (var item in buffer)
            if (item.Equals(value))
                return true;
        return false;
    }
}