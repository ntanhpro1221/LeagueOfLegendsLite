using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(UpdateItemActiveRequestSystemGroup))]
public partial struct UpdateItemActiveRequestSystem : ISystem {
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
    [WithPresent(typeof(ActiveItemWithoutState_Request))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public AllItemData allItem;
        public NetworkTick curTick;

        [BurstCompile]
        private void Execute(
            ref ItemActiveRequestData                    request
          , PlayerInputAspectRO                          input
          , ItemSlotsAspectRO                            itemSlots
          , ActiveItemCostSourceAspect                   costSource
          , EnabledRefRW<ActiveItemWithoutState_Request> withoutStateRequest) {
            // First: reset request
            request.haveRequestNewState = withoutStateRequest.ValueRW = false;

            // Second: check all activable item that requires new state
            foreach (var slot in Strum.SlotItem.Indexes) {
                // Check input
                if (!input.GetEvent_WithData(slot)) continue;

                // Check exist and activable type
                if (!itemSlots.IsActivable(slot, allItem)) continue;
                ref var itemStatic  = ref itemSlots.GetItemDataUnsafe(slot, allItem);
                var     itemDynamic = itemSlots.Slots[slot];

                // Check min level
                if (itemStatic.HaveLevel && itemDynamic.level == 0) continue;
                int levelIndex = itemDynamic.CalcSafeLevelIndex();

                // Check cooldown
                if (itemDynamic.common.doneAtTick.IsValid
                 && itemDynamic.common.doneAtTick.IsNewerThan(curTick)) continue;

                // Check activation cost
                if (!itemStatic.activeCost[levelIndex].IsEnough(costSource)) continue;

                // Check special cond
                if (itemDynamic.common.notSatisSpecialCond) continue;

                // Check activation condition 
                if (!itemStatic.activeCondition.CheckCondOf(input.Input.curCondition)) continue;

                var requireNewState = itemStatic.activeSettings.isRequireNewState;
                request.PushRequest(slot
                  , itemStatic.cooldownTick[levelIndex]
                  , itemStatic.activeCost[levelIndex]
                  , requireNewState);

                if (!requireNewState) withoutStateRequest.ValueRW = true;

                break;
            }
        }
    }
}