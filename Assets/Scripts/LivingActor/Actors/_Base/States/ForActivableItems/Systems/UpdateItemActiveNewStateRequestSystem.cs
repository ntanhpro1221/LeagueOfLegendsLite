using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(Between_CopyCommand_PredictedFixed_SystemGroup))]
public partial struct UpdateItemActiveNewStateRequestSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var curTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

        foreach (var (
            request
          , input
          , prevCode
          , itemsStatic
          , itemsDynamic
          , costSource
            ) in SystemAPI
            .Query<
                RefRW<ItemActiveNewStateRequestData>
              , RefRO<PlayerInputData>
              , RefRO<PlayerTrigger.PrevCode>
              , RefRO<AllActivableItemData>
              , DynamicBuffer<ActivableItemBonusBuffer>
              , ActiveItemCostSourceAspect
            >().WithAll<
                Simulate
            >()) {
            // First: reset request
            request.ValueRW.Reset();

            // Second: check all activable item that requires new state
            for (int i = 0; i < PlayerTrigger.ITEM_COUNT; ++i) {
                var key = (PlayerTrigger.Item)i;

                // Check input
                if (!input.ValueRO.GetEvent_WithData(prevCode.ValueRO, key)) continue;

                // Check exist and activable type
                if (!itemsStatic.ValueRO.IsActivable(key)) continue;
                ref var itemStatic  = ref itemsStatic.ValueRO[key];
                var     itemDynamic = itemsDynamic[i];

                // Check require new state type
                if (!itemStatic.activeSettings.isRequireNewState) continue;
                
                // Check min level
                if (itemStatic.maxLevel != 0 && itemDynamic.level == 0) continue;

                // Check cooldown
                if (itemDynamic.doneAtTick.IsValid
                 && itemDynamic.doneAtTick.IsNewerThan(curTick)) continue;

                // Check activation cost
                if (!itemStatic.activeCost[itemDynamic.level].IsEnough(costSource)) continue;

                // Check activation condition 
                if (!itemStatic.activeCondition.CheckOK(input.ValueRO.curCondition)) continue;

                request.ValueRW.PushRequest(key
                  , itemStatic.cooldownTick[itemDynamic.level]
                  , itemStatic.activeCost[itemDynamic.level]);
                break;
            }
        }
    }
}