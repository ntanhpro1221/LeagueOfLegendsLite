using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleActivableItemDataSystemGroup))]
public partial struct UpgradeSkillSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(
            PlayerInputAspectRO input
          , ref LevelData       level
          , in  SkillsData      skills
          , ref ItemSlotsData   slots) {
            if (!input.GetEvent_WithData(InputRequestId.UpgradeSkill)) return;
            ref var availableSkillPoint = ref level.availableSkillPoint;

            if (availableSkillPoint <= 0) return;
            var     skillToUpgrade = input.Input.requestData.skillToUpgrade;
            ref var curSkillLevel  = ref slots.data.ValueRW(skillToUpgrade).level;

            if (skills[skillToUpgrade].maxLevel <= curSkillLevel) return;

            ++curSkillLevel;
            --availableSkillPoint;
        }
    }
}