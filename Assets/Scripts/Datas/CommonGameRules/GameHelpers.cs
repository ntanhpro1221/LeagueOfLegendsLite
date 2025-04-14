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
        in LocalTransform targetTrans
      , in LocalTransform yourTrans
      , float             yourRange
      , float             targetRadius) =>
        yourRange + targetRadius
      < math.length((targetTrans.Position - yourTrans.Position).WithoutY());

    [BurstCompile]
    public static bool IsTargetExists(
        in Entity                      entity
      , in EntityStorageInfoLookup     entityLookup
      , in ComponentLookup<Selectable> selectLookup) =>
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        entityLookup.Exists(entity)
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
     && selectLookup.HasComponent(entity)
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
     && selectLookup.IsComponentEnabled(entity);

    [BurstCompile]
    public static void AssignLinearVelocity(ref PhysicsVelocity velocity, in float3 linear, bool controlYAxis) {
        if (controlYAxis) velocity.Linear = linear;
        else velocity.Linear.AssignKeepY(linear);
    }
}