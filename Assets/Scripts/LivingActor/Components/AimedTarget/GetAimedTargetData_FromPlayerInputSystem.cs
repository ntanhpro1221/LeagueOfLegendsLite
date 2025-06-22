using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateBefore(typeof(StateMachineSystemGroup))]
public partial struct GetAimedTargetData_FromPlayerInputSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job().ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [WithNone(typeof(NetworkDestroyedTag))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref AimedTargetData target, in PlayerInputData inputData) {
            target.target = inputData.requestData.attackTarget;
        }
    }
}