using Unity.Burst;
using Unity.Mathematics;

public static class mathHelpers {
    public static float3 EulerDiff(quaternion current, quaternion target)
        => math.Euler(math.mul(target, math.inverse(current)));
}