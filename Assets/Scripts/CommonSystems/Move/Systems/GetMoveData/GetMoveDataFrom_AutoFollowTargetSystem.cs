using System;
using Pathfinding.ECS;
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

        state.Dependency = new DirtyJob {
            locTransLookup = locTransLookup, statsLookup = statsLookup, attackRangeId = statsId[StatsType.AttackRange], unitRadiusId = statsId[StatsType.UnitRadius]
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new Job {
            locTransLookup       = locTransLookup
          , takeDamageSpotLookup = takeDamageSpotLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate))]
    [WithNone(
        typeof(NetworkDestroyedTag)
      , typeof(DestinationPoint))]
    public partial struct DirtyJob : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] public BufferLookup<StatsBuffer>       statsLookup;

        public int attackRangeId;
        public int unitRadiusId;

        public void Execute(
            in  LocalTransform   locTrans
          , ref AutoFollowTarget autoFollow
          , in  AimedTargetData  target
          , in  Entity           entity) {
            if (autoFollow.followMethod != AutoFollowTarget.Method.SmartAttack) return;

            // Just log warning in main job below
            if (!locTransLookup.HasComponent(target.target)) return;

            float3 rawTargetPos = locTransLookup[target.target].Position;
            autoFollow.tmpTargetRadius = statsLookup[target.target][unitRadiusId].value;
            autoFollow.tmpYourRange    = statsLookup[entity][attackRangeId].value;
            autoFollow.tmpReachableTargetPos = AstarPath.active.GetNearest(
                math.lerp(
                    rawTargetPos      // Dont need .WithoutY()
                  , locTrans.Position // Dont need .WithoutY()
                  , autoFollow.tmpTargetRadius / GameHelpers.DistanceXZ(rawTargetPos, locTrans.Position))
              , NNConstraintHub.ClosestAsSeenFromAbove).position;
            autoFollow.tmpCurDirToTarget = (rawTargetPos - autoFollow.tmpReachableTargetPos).WithoutY();
        }
    }

    [WithAll(
        typeof(Simulate))]
    [WithNone(
        typeof(NetworkDestroyedTag)
      , typeof(DestinationPoint))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] public ComponentLookup<TakeDamageSpot> takeDamageSpotLookup;

        [BurstCompile]
        public void Execute(
            in AutoFollowTarget autoFollow
          , in AimedTargetData  target
          , MoveRequesterAspect moveRequester
          , in LocalTransform   locTrans) {
            switch (autoFollow.followMethod) {
                case AutoFollowTarget.Method.Straight:
                    moveRequester.MoveStraightTo(locTransLookup[target.target]
                        .TransformPoint(takeDamageSpotLookup.HasComponent(target.target)
                            ? takeDamageSpotLookup[target.target].spot
                            : float3.zero)
                        .Quantizate3());
                    return;

                case AutoFollowTarget.Method.SmartAttack:
                    if (!locTransLookup.HasComponent(target.target)) {
                        Debug.LogWarning($"NGDtuanh: target not exists {target.target} (May be relative to predicted spawn ghost)");
                        return;
                    }

                    float3 rawTargetPos = locTransLookup[target.target].Position;

                    // NEED TO RECALCULATE PATH
                    if (!moveRequester.HandlingTrigger && ( // There is no handling path now
                        // Not have a path
                        moveRequester.WaypointIsEmpty
                        // End point of the current path cannot reach target
                     || GameHelpers.IsTargetOutOfRange(
                            rawTargetPos, moveRequester.WaypointDestination
                          , autoFollow.tmpYourRange, autoFollow.tmpTargetRadius)
                        // The difference between previous and current (direction to target) is large and need update
                     || MAX_DIR_DEGREE_ERROR < Vector3.Angle(autoFollow.tmpCurDirToTarget
                          , (rawTargetPos - moveRequester.WaypointDestination).WithoutY())))
                        moveRequester.MoveSmartTo(autoFollow.tmpReachableTargetPos.Quantizate3(), locTrans);

                    return;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}