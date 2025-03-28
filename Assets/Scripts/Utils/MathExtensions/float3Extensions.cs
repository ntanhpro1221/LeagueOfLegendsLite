using Unity.Mathematics;

public static class float3Extensions {
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
}