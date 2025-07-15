using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(ActiveItemWithoutStateSystemGroup), OrderFirst = true)]
public partial struct AnalyzeActiveItemWithoutStateRequestSystem : ISystem {
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

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public NetworkTick curTick;

        [BurstCompile]
        private void Execute(
            ref ItemSlotsData                            slots
          , in  ItemActiveRequestData                    requestData
          , CCAspectRO                                   cc
          , ActiveItemCostSourceAspect                   costSource
          , EnabledRefRW<ActiveItemWithoutState_Request> requestTrigger) {
            if (cc.Disable.ActiveItem != 0) {
                requestTrigger.ValueRW = false;
                return;
            }
            
            // Set cooldown
            slots.data.ValueRW(requestData.item).common.UpdateCooldownAfterActive(curTick, requestData.cooldownTick);

            // Apply cost
            requestData.cost.ApplyCost(costSource);
        }
    }
}