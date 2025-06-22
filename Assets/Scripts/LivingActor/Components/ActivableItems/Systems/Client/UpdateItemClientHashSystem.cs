using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(HandleItemClientUISystemGroup), OrderLast = true)]
public partial struct UpdateItemClientHashSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref ItemSlotsData data) {
            data.clientHash = data.serverHash;
        }
    }
}