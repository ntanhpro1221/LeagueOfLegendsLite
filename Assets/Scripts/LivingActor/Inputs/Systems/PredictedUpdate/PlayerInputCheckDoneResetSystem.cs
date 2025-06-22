using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(InputPredictedUpdateSystemGroup))]
public partial struct PlayerInputCheckDoneResetSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job().Schedule(state.Dependency);
    }

    [BurstCompile]
    public partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref PlayerInputData inputData, EnabledRefRW<PlayerInputResetting> resettingTag) {
            if (inputData.GetEvent_Only(InputRequestId.DoneReset))
                resettingTag.ValueRW = false;
        }
    }
}