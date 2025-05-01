using Unity.Burst;
using Unity.Mathematics;

public static class mathHelpers {
    public static float3 EulerDiff(quaternion current, quaternion target)
        => math.Euler(math.mul(target, math.inverse(current)));
    public static float    Sqr(this float    source) => source * source;
    public static int      Sqr(this int      source) => source * source;
    public static float_Q3 Sqr(this float_Q3 source) => source * source;
}