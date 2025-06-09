using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleActivableItemDataSystemGroup))]
public partial struct UpgradeSkillSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
            input
          , level
          , itemsStatic
          , itemsDynamic
            ) in SystemAPI
            .Query<
                PlayerInputAspectRO
              , RefRW<LevelData>
              , AllActivableItemData
              , DynamicBuffer<ActivableItemBonusBuffer>
            >().WithAll<
                Simulate
            >()) {
            if (!input.GetEvent_WithData(PlayerTrigger.Other.UpgradeSkill)) continue;
            ref var availableSkillPoint = ref level.ValueRW.availableSkillPoint;

            if (availableSkillPoint <= 0) continue;
            var     skillToUpgrade = input.Input.skillToUpgrade;
            ref var curSkillLevel  = ref itemsDynamic.ElementAt((int)skillToUpgrade).level;

            if (itemsStatic[skillToUpgrade].maxLevel <= curSkillLevel) continue;

            ++curSkillLevel;
            --availableSkillPoint;
        }
    }
}