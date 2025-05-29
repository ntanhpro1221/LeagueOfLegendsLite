using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
public partial struct Update_PlayerInput_Trigger_PrevCode_System : ISystem {
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
        public void Execute(
            in  PlayerInputData     inputData
          , ref PlayerInputPrevCode prevCode) {
            if (!inputData.tickVersion.IsValid(curTick)) return;

            prevCode.Code = inputData.triggers.Code;
        }
    }
}