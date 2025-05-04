using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Just follow <see cref="MinionFixedPathBuffer"/> now
/// </summary>
[UpdateInGroup(typeof(ActorAIControlSystemGroup))]
public partial struct MinionControlSystem : ISystem {
    [ReadOnly] private ComponentLookup<Selectable> selectLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        selectLookup = SystemAPI.GetComponentLookup<Selectable>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        selectLookup.Update(ref state);

        state.Dependency = new SeekTargetJob {
            selectLookup = selectLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MinionTag))]
    [WithPresent(
        typeof(AggroAnchor)
      , typeof(AggroDisabling))]
    [BurstCompile]
    private partial struct SeekTargetJob : IJobEntity {
        [ReadOnly] public ComponentLookup<Selectable> selectLookup;

        [BurstCompile]
        public void Execute(
            ref AimedTargetData                       aimedTarget
          , in  AllyBeAttackedData                    allyBeAttacked
          , in  DynamicBuffer<DetectedChampionBuffer> detectedChampion
          , in  LocalTransform                        locTrans
          , ref AggroAnchor                           aggroAnchor
          , EnabledRefRW<AggroAnchor>                 anchorEnable
          , EnabledRefRO<AggroDisabling>              aggroDisable) {
            // When my champ be attack by enemy champ
            if (GameHelpers.IsTargetExists(allyBeAttacked.champByChamp, selectLookup)
                 && detectedChampion.Contains(allyBeAttacked.champByChamp))
                // bypass aggroDisable check
                AimToChamp(
                    allyBeAttacked.champByChamp
                  , locTrans
                  , ref aimedTarget
                  , ref aggroAnchor
                  , anchorEnable);
            // Champion is current target but aggro is disabled
            else if (
                aimedTarget.targetIsChampion
             && aggroDisable.ValueRO) 
                aimedTarget.target = Entity.Null;
        }
    }

    [BurstCompile]
    public static void AimToChamp(
        in  Entity                    target
      , in  LocalTransform            locTrans
      , ref AimedTargetData           aimedTarget
      , ref AggroAnchor               aggroAnchor
      , in  EnabledRefRW<AggroAnchor> anchorEnable) {
        aimedTarget.target           = target;
        aggroAnchor.anchor           = locTrans.Position.Quantizate3();
        anchorEnable.ValueRW         = true;
        aimedTarget.targetIsChampion = true;
    }
}