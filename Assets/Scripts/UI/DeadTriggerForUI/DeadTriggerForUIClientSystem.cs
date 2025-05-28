using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
public partial struct DeadTriggerForUIClientSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithPresent(typeof(DeadState))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(
            EnabledRefRO<DeadState>  curDeadState
          , ref DeadTriggerForUIData deadTriggerUI) {
            deadTriggerUI.Update(curDeadState.ValueRO);
        }
    }
}