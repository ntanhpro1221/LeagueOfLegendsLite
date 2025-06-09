using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleBountySystemGroup), OrderLast = true)]
public partial struct TurnOffBountyTriggerSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(BountyAspectRW bounty) {
            bounty.TurnOff();
        }
    }
}