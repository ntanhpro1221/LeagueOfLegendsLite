using NGDtuanh.Entities.StateMachine;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateBefore(typeof(StateMachineSystemGroup))]
public partial struct GetAimedTargetData_FromPlayerInputSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
                aimedTarget
              , playerInput)
            in SystemAPI.Query<
                    RefRW<AimedTargetData>
                  , RefRO<PlayerInputData>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>())
            if (playerInput.ValueRO.attackEvent.IsSet)
                aimedTarget.ValueRW.target = playerInput.ValueRO.attackTarget;
    }
}