using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorAIControlSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateBefore(typeof(MonsterControlSystem))]
public partial struct MonsterLeashTogglerSystem : ISystem {
    [ReadOnly] private ComponentLookup<Selectable> selectLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        selectLookup = SystemAPI.GetComponentLookup<Selectable>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        selectLookup.Update(ref state);

        state.Dependency = new DisableJob {
            selectLookup = selectLookup
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new EnableJob()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag))]
    [WithPresent(
        typeof(MonsterLeashDisabling))]
    [BurstCompile]
    private partial struct DisableJob : IJobEntity {
        [ReadOnly] public ComponentLookup<Selectable> selectLookup;

        [BurstCompile]
        public void Execute(
            ref MonsterLeashAnchor                   leashAnchorData
          , ref MonsterLeashDisabling                leashDisableData
          , EnabledRefRW<MonsterLeashAnchor>         leashAnchorTrigger
          , EnabledRefRW<MonsterLeashDisabling>      leashDisableTrigger
          , in LocalTransform                        locTrans
          , in MonsterControlFactor                  controlFactor
          , in DynamicBuffer<DetectedChampionBuffer> detectedChamp
          , AimedTargetAspectRO                      aimedTarget) {
            if ( // Not have target in leash range and current target is not exist anymore or
                (detectedChamp.IsEmpty && !aimedTarget.IsTargetExists(selectLookup))
                // has moved too far, it's time to return to anchor point
             || controlFactor.leashRangeSqr < GameHelpers.DistanceXZ_Sqr(locTrans.Position
                  , leashAnchorData.anchorPos)) {
                leashAnchorTrigger.ValueRW  = false;
                leashDisableTrigger.ValueRW = true;

                leashDisableData.nextRegenTick = new NetworkTick(0);
            }
        }
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag))]
    [WithPresent(typeof(MonsterLeashAnchor))]
    [BurstCompile]
    private partial struct EnableJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            in MonsterLeashAnchor               anchorData
          , EnabledRefRW<MonsterLeashDisabling> leashDisableTrigger
          , in  LocalTransform                  locTrans) {
            if ( // Returned to anchor point and
                1 > GameHelpers.DistanceXZ_Sqr(anchorData.anchorPos, locTrans.Position))
                leashDisableTrigger.ValueRW = false;
        }
    }
}