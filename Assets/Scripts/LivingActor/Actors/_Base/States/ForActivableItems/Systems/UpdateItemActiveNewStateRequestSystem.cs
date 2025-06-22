using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(Between_CopyCommand_PredictedFixed_SystemGroup))]
public partial struct UpdateItemActiveNewStateRequestSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllItemData>();
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            allItem = SystemAPI.GetSingleton<AllItemData>()
          , curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public AllItemData allItem;
        public NetworkTick curTick;

        [BurstCompile]
        public void Execute(
            ref ItemActiveNewStateRequestData request
          , PlayerInputAspectRO               input
          , ItemSlotsAspectRO                 itemSlots
          , ActiveItemCostSourceAspect        costSource) {
            // First: reset request
            request.Reset();

            // Second: check all activable item that requires new state
            foreach (var slot in Strum.SlotItem.Indexes) {
                // Check input
                if (!input.GetEvent_WithData(slot)) continue;

                // Check exist and activable type
                if (!itemSlots.IsActivable(slot, allItem)) continue;
                ref var itemStatic  = ref itemSlots.GetItemDataUnsafe(slot, allItem);
                var     itemDynamic = itemSlots.Slots[slot];

                // Check require new state type
                if (!itemStatic.activeSettings.isRequireNewState) continue;

                // Check min level
                if (itemStatic.haveLevel && itemDynamic.level == 0) continue;
                int levelIndex = itemStatic.CalcLevelIndex(itemDynamic.level);

                // Check cooldown
                if (itemDynamic.common.doneAtTick.IsValid
                 && itemDynamic.common.doneAtTick.IsNewerThan(curTick)) continue;

                // Check activation cost
                if (!itemStatic.activeCost[levelIndex].IsEnough(costSource)) continue;

                // Check activation condition 
                if (!itemStatic.activeCondition.CheckOK(input.Input.curCondition)) continue;

                request.PushRequest(slot
                  , itemStatic.cooldownTick[levelIndex]
                  , itemStatic.activeCost[levelIndex]);

                break;
            }
        }
    }
}