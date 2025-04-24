using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(DestroyNetworkEntitySystemGroup))]
public partial struct DestroyNetworkEntityServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                NetworkDestroyedTag
              , Simulate>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job {
            ecbParallel = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter()
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(NetworkDestroyedTag))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecbParallel;

        [BurstCompile]
        public void Execute(in Entity entity, [EntityIndexInQuery] int queryId) {
            ecbParallel.DestroyEntity(queryId, entity);
        }
    }
}