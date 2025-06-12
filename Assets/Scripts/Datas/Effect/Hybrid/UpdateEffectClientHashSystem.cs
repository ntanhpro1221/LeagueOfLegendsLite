using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleEffectClientUISystemGroup), OrderLast = true)]
public partial struct UpdateEffectClientHashSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref EffectBufferHashData data) {
            data.clientHash = data.serverHash;
        }
    }
}