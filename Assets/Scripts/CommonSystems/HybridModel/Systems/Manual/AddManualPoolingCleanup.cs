using Unity.Burst;
using Unity.Entities;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct AddManualPoolingCleanup : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<ManualPoolingHybridModel>()
            .WithNone<ManualPoolingHybridModel_Cleanup>()
            .Build());
    }

    public void OnUpdate(ref SystemState state) {
        new Job {
            ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
        }.Run();
    }

    [WithAll(typeof(ManualPoolingHybridModel))]
    [WithNone(typeof(ManualPoolingHybridModel_Cleanup))]
    public partial struct Job : IJobEntity {
        public EntityCommandBuffer ecb;

        public void Execute(in Entity entity) =>
            ecb.AddComponent<ManualPoolingHybridModel_Cleanup>(entity);
    }
}