using System;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// Just analysis for skill at the moment.
/// </summary>
[UpdateInGroup(typeof(StateItemActiveAnalysisSystemGroup))]
public partial struct StateItemActiveAnalysisSystem_Base : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(ItemActiveAnalyzingState))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public NetworkTick curTick;

        [BurstCompile]
        public void Execute(
            ref CommonItemActiveStateData               commonStateData
          , ref DynamicBuffer<ActivableItemBonusBuffer> itemDynamic
          , in  ItemActiveNewStateRequestData           requestData
          , in  PlayerInputData                         input
          , ActorSharedStateAspect                      stateSetter
          , ActiveItemCostSourceAspect                  costSource) {
            // Turn off state
            stateSetter.UnsetItemActiveAnalyzing();

            // Set common data
            commonStateData.SetInputData(input);

            // Set cooldown
            itemDynamic.ElementAt((int)requestData.item).UpdateCooldownAfterActive(curTick, requestData.cooldownTick);

            // Apply cost
            requestData.cost.ApplyCost(costSource);

            // State translate
            switch (requestData.item) {
                case PlayerTrigger.Item.Skill_Q: stateSetter.SetSkill_Q(); break;
                case PlayerTrigger.Item.Skill_W: stateSetter.SetSkill_W(); break;
                case PlayerTrigger.Item.Skill_E: stateSetter.SetSkill_E(); break;
                case PlayerTrigger.Item.Skill_R: stateSetter.SetSkill_R(); break;

                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}