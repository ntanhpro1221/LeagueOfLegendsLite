using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(UpdateItemSpecialCondSystemGroup))]
public partial struct UpdateItemSpecialCondSystem_Ashe_Q : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        // Just run when ashe is in battle
        state.RequireForUpdate<AsheTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
            slots
          , effectMap
            ) in SystemAPI
            .Query<
                RefRW<ItemSlotsData>
              , EffectMapAspectRO
            >().WithAll<
                AsheTag
              , Simulate
            >()) {
            ref bool notSatisSpecialCond = ref slots.ValueRW.data.ValueRW(SlotItemId.Skill_Q).common.notSatisSpecialCond;
            notSatisSpecialCond = true;

            if (!effectMap.TryGetFirstEffect(EffectId.AsheSkill_Q_Stack, out var effectData)) continue;

            notSatisSpecialCond = effectData.curStack != effectData.stackingBehaviour.maxStack;
        }
    }
}