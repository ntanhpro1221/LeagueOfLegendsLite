using Unity.Mathematics;
using UnityEngine;

public static class VectorExtensions {
    /// <returns>A new vector with the modified X component.</returns>
    public static Vector3 WithX(this Vector3 source, float x) {
        source.x = x;
        return source;
    }

    /// <returns>A new vector with the modified Y component.</returns>
    public static Vector3 WithY(this Vector3 source, float y) {
        source.y = y;
        return source;
    }

    /// <returns>A new vector with the modified Z component.</returns>
    public static Vector3 WithZ(this Vector3 source, float z) {
        source.z = z;
        return source;
    }

    public static Vector3 WithAddX(this Vector3 source, float x) {
        source.x += x;
        return source;
    }

    public static Vector3 WithAddY(this Vector3 source, float y) {
        source.y += y;
        return source;
    }

    public static Vector3 WithAddZ(this Vector3 source, float z) {
        source.z += z;
        return source;
    }

    /// <returns>A new vector with only the X component preserved (Y and Z set to 0).</returns>
    public static Vector3 JustX(this Vector3 source) {
        source.y = source.z = 0;
        return source;
    }

    /// <returns>A new vector with only the Y component preserved (X and Z set to 0).</returns>
    public static Vector3 JustY(this Vector3 source) {
        source.x = source.z = 0;
        return source;
    }

    /// <returns>A new vector with only the Z component preserved (X and Y set to 0).</returns>
    public static Vector3 JustZ(this Vector3 source) {
        source.x = source.y = 0;
        return source;
    }

    /// <returns>A new vector with the X component set to 0.</returns>
    public static Vector3 WithoutX(this Vector3 source) {
        source.x = 0;
        return source;
    }

    /// <returns>A new vector with the Y component set to 0.</returns>
    public static Vector3 WithoutY(this Vector3 source) {
        source.y = 0;
        return source;
    }

    /// <returns>A new vector with the Z component set to 0.</returns>
    public static Vector3 WithoutZ(this Vector3 source) {
        source.z = 0;
        return source;
    }

    public static float4_Q3 Quantizate3(this Vector4 source)
        => (float4_Q3)source;

    public static float3_Q3 Quantizate3(this Vector3 source)
        => (float3_Q3)source;

    public static bool IsPositiveInfinity_Any(this Vector3 source) =>
        float.IsPositiveInfinity(source.x)
     || float.IsPositiveInfinity(source.y)
     || float.IsPositiveInfinity(source.z);

    public static bool IsPositiveInfinity_All(this Vector3 source) =>
        float.IsPositiveInfinity(source.x)
     && float.IsPositiveInfinity(source.y)
     && float.IsPositiveInfinity(source.z);

    public static bool IsPositiveInfinity_X(this Vector3 source) =>
        float.IsPositiveInfinity(source.x);
}