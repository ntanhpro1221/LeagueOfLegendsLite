using System;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

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
            ref ItemCommonStateData     commonStateData
          , ref ItemSlotsData                 slots
          , in  ItemActiveNewStateRequestData requestData
          , in  PlayerInputData               input
          , ActorSharedStateAspect            stateSetter
          , ActiveItemCostSourceAspect        costSource) {
            // Turn off state
            stateSetter.UnsetItemActiveAnalyzing();

            // Set cooldown
            slots.data.ValueRW(requestData.item).common.UpdateCooldownAfterActive(curTick, requestData.cooldownTick);

            // Apply cost
            requestData.cost.ApplyCost(costSource);

            // State translate
            switch (requestData.item) {
                case SlotItemId.Skill_Q: stateSetter.SetSkill_Q(); break;
                case SlotItemId.Skill_W: stateSetter.SetSkill_W(); break;
                case SlotItemId.Skill_E: stateSetter.SetSkill_E(); break;
                case SlotItemId.Skill_R: stateSetter.SetSkill_R(); break;
                case >= Strum.SlotItem.First_Item and <= Strum.SlotItem.Last_Item:
                    // Spell is also considered as a common item
                case >= Strum.SlotItem.First_Spell and <= Strum.SlotItem.Last_Spell:
                    stateSetter.SetItemCommon();
                    break;

                default: throw new ArgumentOutOfRangeException();
            }

            // Set common data
            commonStateData.SetData(input, requestData.item);
        }
    }
}