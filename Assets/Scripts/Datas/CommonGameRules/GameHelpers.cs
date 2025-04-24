using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public static class GameHelpers {
    [BurstCompile]
    public static NetworkTick CalcRespawnTick_Champion(
        in DynamicBuffer<BaseRespawnWaitTimeBuffer> BRW
      , in NetworkTime                              networkTime
      , int                                         simulationTickRate
      , int                                         curLevel) {

        NetworkTick respawnTick = networkTime.ServerTick;
        respawnTick.Add((uint)(BRW[curLevel - 1].value * simulationTickRate));
        return respawnTick;
    }

    [BurstCompile]
    public static bool IsTargetOutOfRange(
        in float3 targetPos
      , in float3 yourPos
      , float     yourRange
      , float     targetRadius) =>
        yourRange + targetRadius
      < math.length((targetPos - yourPos).WithoutY());

    [BurstCompile]
    public static bool IsTargetExists(
        in Entity                      entity
      , in ComponentLookup<Selectable> selectLookup) =>
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        selectLookup.HasComponent(entity)
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
     && selectLookup.IsComponentEnabled(entity);

    [BurstCompile]
    public static void AssignLinearVelocity(ref PhysicsVelocity velocity, in float3 linear, bool controlYAxis) {
        if (controlYAxis) velocity.Linear = linear;
        else velocity.Linear.AssignKeepY(linear);
    }

    public static float DistanceXZ_Sqr(float3 alice, float3 bob)
        => math.lengthsq((alice - bob).WithoutY());

    public static float DistanceXZ(float3 alice, float3 bob)
        => math.length((alice - bob).WithoutY());
}