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

    public static bool Empty<TBuffer>(
        this DynamicBuffer<TBuffer> buffer)
        where TBuffer : unmanaged
        => buffer.Length == 0;
    
    public static ref TBuffer BackRW<TBuffer>(
        this DynamicBuffer<TBuffer> buffer)
        where TBuffer : unmanaged
        => ref buffer.ElementAt(buffer.Length - 1);

    public static ref readonly TBuffer BackRO<TBuffer>(
        this DynamicBuffer<TBuffer> buffer)
        where TBuffer : unmanaged
        => ref buffer.ElementAt(buffer.Length - 1);

    public static ref TBuffer FrontRW<TBuffer>(
        this DynamicBuffer<TBuffer> buffer)
        where TBuffer : unmanaged
        => ref buffer.ElementAt(0);

    public static ref readonly TBuffer FrontRO<TBuffer>(
        this DynamicBuffer<TBuffer> buffer)
        where TBuffer : unmanaged
        => ref buffer.ElementAt(0);

    public static TBuffer PopBack<TBuffer>(
        this DynamicBuffer<TBuffer> buffer)
        where TBuffer : unmanaged {
        var result = buffer.BackRO();
        buffer.RemoveAt(buffer.Length - 1);
        return result;
    }
    
    public static TBuffer PopFront<TBuffer>(
        this DynamicBuffer<TBuffer> buffer)
        where TBuffer : unmanaged {
        var result = buffer.FrontRO();
        buffer.RemoveAt(0);
        return result;
    }
}