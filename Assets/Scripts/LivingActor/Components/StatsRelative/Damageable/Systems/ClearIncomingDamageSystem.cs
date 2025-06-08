using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleIncomingDamageSystemGroup), OrderLast = true)]
public partial struct ClearIncomingDamageSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(HealthData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref DynamicBuffer<IncomingDamageBuffer> incomingDamage) {
            incomingDamage.Clear();
        }
    }
}