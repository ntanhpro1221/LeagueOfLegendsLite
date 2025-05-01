using Unity.Mathematics;
using UnityEngine;

public static class floatExtensions {
    /// <returns>A new vector with the modified X component.</returns>
    public static float3 WithX(this float3 source, float x) {
        source.x = x;
        return source;
    }

    /// <returns>A new vector with the modified Y component.</returns>
    public static float3 WithY(this float3 source, float y) {
        source.y = y;
        return source;
    }

    /// <returns>A new vector with the modified Z component.</returns>
    public static float3 WithZ(this float3 source, float z) {
        source.z = z;
        return source;
    }

    public static float3 WithAddX(this float3 source, float x) {
        source.x += x;
        return source;
    }

    public static float3 WithAddY(this float3 source, float y) {
        source.y += y;
        return source;
    }

    public static float3 WithAddZ(this float3 source, float z) {
        source.z += z;
        return source;
    }

    /// <returns>A new vector with only the X component preserved (Y and Z set to 0).</returns>
    public static float3 JustX(this float3 source) {
        source.y = source.z = 0;
        return source;
    }

    /// <returns>A new vector with only the Y component preserved (X and Z set to 0).</returns>
    public static float3 JustY(this float3 source) {
        source.x = source.z = 0;
        return source;
    }

    /// <returns>A new vector with only the Z component preserved (X and Y set to 0).</returns>
    public static float3 JustZ(this float3 source) {
        source.x = source.y = 0;
        return source;
    }

    /// <returns>A new vector with the X component set to 0.</returns>
    public static float3 WithoutX(this float3 source) {
        source.x = 0;
        return source;
    }

    /// <returns>A new vector with the Y component set to 0.</returns>
    public static float3 WithoutY(this float3 source) {
        source.y = 0;
        return source;
    }

    /// <returns>A new vector with the Z component set to 0.</returns>
    public static float3 WithoutZ(this float3 source) {
        source.z = 0;
        return source;
    }

    public static void AssignKeepX(this ref float3 source, float3 value)
        => (source.y, source.z) = (value.y, value.z);

    public static void AssignKeepY(this ref float3 source, float3 value)
        => (source.x, source.z) = (value.x, value.z);

    public static void AssignKeepZ(this ref float3 source, float3 value)
        => (source.x, source.y) = (value.x, value.y);

    public static float4_Q3 Quantizate3(this float4 source)
        => (float4_Q3)source;

    public static float3_Q3 Quantizate3(this float3 source)
        => (float3_Q3)source;

    public static float_Q3 Quantizate3(this float source)
        => (float_Q3)source;

    public static float LengthSqr(this floatXZ_Q3 source) =>
        math.square((float)source.x / floatXZ_Q3.MULTIPLIER)
      + math.square((float)source.z / floatXZ_Q3.MULTIPLIER);

}