using Unity.Burst;
using Unity.Entities;

public partial struct InitEffectMapSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<EffectBuffer>()
            .WithNone<EffectMap>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        new Job {
            ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
        }.Run();
    }

    [WithAll(typeof(EffectBuffer))]
    [WithNone(typeof(EffectMap))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public EntityCommandBuffer ecb;

        [BurstCompile]
        private void Execute(in Entity entity) => ecb.AddComponent(entity, EffectMap.Construct());
    }
}