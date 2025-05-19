using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorAIControlSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct MonsterControlSystem : ISystem {
    [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
    [ReadOnly] private ComponentLookup<BeBeaten>       beatenLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        selectLookup = SystemAPI.GetComponentLookup<Selectable>(
            isReadOnly: true);
        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
        beatenLookup = SystemAPI.GetComponentLookup<BeBeaten>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        selectLookup.Update(ref state);
        locTransLookup.Update(ref state);
        beatenLookup.Update(ref state);

        state.Dependency = new SeekNearestTargetJob {
            selectLookup   = selectLookup
          , locTransLookup = locTransLookup
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new LeashWhenBeBeatenJob()
            .ScheduleParallel(state.Dependency);

        state.Dependency = new LeashWhenUnderlingBeBeatenJob {
            selectLookup = selectLookup
          , beatenLookup = beatenLookup
        }.ScheduleParallel(state.Dependency);

        // this UpdateBeBeatenFromCurTarget must be updated in this order
        // otherwise LeashWhenLeaderBeBeatenJob cannot detect new target from LeashWhenUnderlingBeBeatenJob
        state.Dependency = new UpdateBeBeatenFromCurTarget()
            .ScheduleParallel(state.Dependency);

        state.Dependency = new LeashWhenLeaderBeBeatenJob {
            selectLookup = selectLookup
          , beatenLookup = beatenLookup
        }.ScheduleParallel(state.Dependency);
    }


    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag)
      , typeof(MonsterLeashAnchor))]
    [WithDisabled(
        typeof(MonsterLeashDisabling))]
    [WithPresent(
        typeof(AttackState))]
    [BurstCompile]
    private partial struct SeekNearestTargetJob : IJobEntity {
        private const float CHANGE_TARGET_DIS_THRESHOLD = 100;

        [ReadOnly] public ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;

        [BurstCompile]
        public void Execute(
            ref AimedTargetData                      targetData
          , EnabledRefRO<AttackState>                attacking
          , in AttackStateData                       attackData
          , in DynamicBuffer<DetectedChampionBuffer> detectedChamp
          , in LocalTransform                        locTrans) {
            // In attack state but not perform a real attack yet
            if (attacking.ValueRO && !attackData.isAttacked) return;

            float disCurTargetSqr = GameHelpers.IsTargetExists(targetData.target, selectLookup)
                ? GameHelpers.DistanceXZ_Sqr(locTrans.Position
                  , locTransLookup[targetData.target].Position) - CHANGE_TARGET_DIS_THRESHOLD
                : float.PositiveInfinity;

            foreach (var champ in detectedChamp) {
                float disSqr = GameHelpers.DistanceXZ_Sqr(locTrans.Position
                  , locTransLookup[champ.entity].Position);
                if (disSqr > disCurTargetSqr) continue;

                disCurTargetSqr   = disSqr;
                targetData.target = champ.entity;
            }
        }
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag))]
    [WithDisabled( // When not trace anyone and not in disable leash state
        typeof(MonsterLeashDisabling)
      , typeof(MonsterLeashAnchor))]
    [BurstCompile]
    private partial struct LeashWhenBeBeatenJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            MonsterTargetSetterAspect targetSetter
          , in BeBeaten               beBeaten) {
            targetSetter.SetTargetUnsafe(beBeaten.source);
        }
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag))]
    [WithDisabled( // When not trace anyone and not in disable leash state
        typeof(MonsterLeashDisabling)
      , typeof(MonsterLeashAnchor)
      , typeof(BeBeaten))] // If be beaten is on, then he would already have target so just skip him
    [BurstCompile]
    private partial struct LeashWhenUnderlingBeBeatenJob : IJobEntity {
        [ReadOnly] public ComponentLookup<Selectable> selectLookup;
        [ReadOnly] public ComponentLookup<BeBeaten>   beatenLookup;

        [BurstCompile]
        public void Execute(
            MonsterTargetSetterAspect                  targetSetter
          , in DynamicBuffer<MonsterMyUnderlingBuffer> underlingBuffer) {
            foreach (var underling in underlingBuffer)
                if (beatenLookup.IsComponentEnabled(underling))
                    if (targetSetter.TrySetTarget(beatenLookup[underling].source, selectLookup))
                        return;
        }
    }

    /// <summary>
    /// Just merge current target, not turn <see cref="BeBeaten"/> on
    /// </summary>
    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag))]
    [WithDisabled(typeof(BeBeaten))]
    [BurstCompile]
    private partial struct UpdateBeBeatenFromCurTarget : IJobEntity {
        [BurstCompile]
        public void Execute(
            in  AimedTargetData    targetData
          , ref BeBeaten           beBeaten
          , EnabledRefRW<BeBeaten> beBeatenTrigger) {
            if (beBeaten.source == targetData.target) return;

            beBeaten.source         = targetData.target;
            beBeatenTrigger.ValueRW = true;
        }
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag))]
    [WithDisabled( // When not trace anyone and not in disable leash state
        typeof(MonsterLeashDisabling)
      , typeof(MonsterLeashAnchor)
      , typeof(BeBeaten))] // If be beaten is on, then he would already have target so just skip him
    [BurstCompile]
    private partial struct LeashWhenLeaderBeBeatenJob : IJobEntity {
        [ReadOnly] public ComponentLookup<Selectable> selectLookup;
        [ReadOnly] public ComponentLookup<BeBeaten>   beatenLookup;

        [BurstCompile]
        public void Execute(
            MonsterTargetSetterAspect targetSetter
          , in MonsterUnderlingData   underlingData) {
            if (beatenLookup.IsComponentEnabled(underlingData.leader))
                targetSetter.TrySetTarget(beatenLookup[underlingData.leader].source, selectLookup);
        }
    }
}