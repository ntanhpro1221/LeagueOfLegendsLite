using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
public partial struct Update_PlayerInput_Trigger_PrevCode_System : ISystem {
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
            in  PlayerInputData        inputData
          , ref PlayerTrigger.PrevCode prevCode) {
            prevCode.Code = inputData.triggers.Code;
        }
    }
}