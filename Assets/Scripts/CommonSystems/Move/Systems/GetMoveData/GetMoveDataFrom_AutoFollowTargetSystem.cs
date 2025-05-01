using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PrepareMoveSystemGroup))]
public partial struct GetMoveDataFrom_AutoFollowTargetSystem : ISystem {
    public const float MAX_DIR_DEGREE_ERROR = 45;

    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
    [ReadOnly] private ComponentLookup<TakeDamageSpot> takeDamageSpotLookup;
    [ReadOnly] private BufferLookup<StatsBuffer>       statsLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate<NetworkTime>();
        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
        takeDamageSpotLookup = SystemAPI.GetComponentLookup<TakeDamageSpot>(
            isReadOnly: true);
        statsLookup = SystemAPI.GetBufferLookup<StatsBuffer>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        locTransLookup.Update(ref state);
        takeDamageSpotLookup.Update(ref state);
        statsLookup.Update(ref state);

        ref var statsId = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;

        state.Dependency = new Job {
            locTransLookup       = locTransLookup
          , takeDamageSpotLookup = takeDamageSpotLookup
          , statsLookup          = statsLookup
          , attackRangeId        = statsId[StatsType.AttackRange]
          , unitRadiusId         = statsId[StatsType.UnitRadius]
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate))]
    [WithNone(
        typeof(NetworkDestroyedTag))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] public ComponentLookup<TakeDamageSpot> takeDamageSpotLookup;
        [ReadOnly] public BufferLookup<StatsBuffer>       statsLookup;

        public int attackRangeId;
        public int unitRadiusId;

        [BurstCompile]
        public void Execute(
            in LocalTransform   locTrans
          , in AutoFollowTarget autoFollow
          , in AimedTargetData  target
          , MoveRequesterAspect moveRequester
          , in Entity           entity) {
            switch (autoFollow.followMethod) {
                case AutoFollowTarget.Method.Straight:
                    moveRequester.MoveStraightTo(locTransLookup[target.target]
                        .TransformPoint(takeDamageSpotLookup.HasComponent(target.target)
                            ? takeDamageSpotLookup[target.target].spot
                            : float3.zero)
                        .Quantizate3());
                    return;

                case AutoFollowTarget.Method.SmartAttack:
                    float3   targetPos    = locTransLookup[target.target].Position;
                    float3   dirToTarget  = (targetPos - locTrans.Position).WithoutY();
                    float_Q3 yourRange    = statsLookup[entity][attackRangeId].value;
                    float_Q3 targetRadius = statsLookup[target.target][unitRadiusId].value;

                    // NEED TO RECALCULATE PATH
                    if (
                        // Not have a path
                        !moveRequester.AlreadyHaveWaypoint
                        // End point of the current path cannot reach target
                     || GameHelpers.IsTargetOutOfRange(
                            targetPos, moveRequester.WaypointDestination
                          , yourRange, targetRadius)
                        // The difference between previous and current (direction to target) is large and need update
                     || MAX_DIR_DEGREE_ERROR < Vector3.Angle(dirToTarget
                          , (targetPos - moveRequester.WaypointDestination).WithoutY())) {
                        moveRequester.MoveSmartTo(math.lerp(
                                targetPos         // Dont need .WithoutY()
                              , locTrans.Position // Dont need .WithoutY()
                              , targetRadius / GameHelpers.DistanceXZ(targetPos, locTrans.Position))
                            .Quantizate3());
                    }

                    return;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}