    using Pathfinding.ECS;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct FollowerEntityFixedPathJob : IJobEntity {
    public const float DES_DIS_TOLERANCE_SQR = 100f;

    public static EntityQueryBuilder TakeMyQuery(EntityQueryBuilder queryBuilder) => queryBuilder
        .WithAllRW<
            DestinationPoint
          , FollowerEntityFixedPathStatus>()
        .WithAll<
            FollowerEntityFixedPathBuffer
          , LocalTransform>();

    [BurstCompile]
    public void Execute(
        ref DestinationPoint                             desSetter
      , ref FollowerEntityFixedPathStatus                pathStatus
      , in  DynamicBuffer<FollowerEntityFixedPathBuffer> pathBuffer
      , in  LocalTransform                               locTrans) {
        var curTarget = pathBuffer[pathStatus.curTargetIndex].pos;

        // Not set path yet
        if (1 < GameHelpers.DistanceXZ_Sqr(desSetter.destination, curTarget)) {
            desSetter.destination = curTarget;
            return;
        }

        // Not reach destination point yet
        if (DES_DIS_TOLERANCE_SQR < GameHelpers.DistanceXZ_Sqr(locTrans.Position, curTarget)) {
            return;
        }

        // Set next target
        desSetter.destination = pathBuffer[
            pathStatus.curTargetIndex = ++pathStatus.curTargetIndex % pathBuffer.Length
        ].pos;
    }
}