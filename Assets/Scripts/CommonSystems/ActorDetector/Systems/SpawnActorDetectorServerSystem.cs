using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct SpawnActorDetectorServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<NeedSpawnActorDetector>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        // Just use BeginSimulationEntityCommandBufferSystem is enough (maybe)
        var ecbParallel = SystemAPI
            .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();

        state.Dependency = new SpawnJob {
            ecbParallel       = ecbParallel
          , isServer          = state.WorldUnmanaged.IsServer()
        }.ScheduleParallel(state.Dependency);

        state.Dependency = new MarkRequestCompletedJob()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct SpawnJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecbParallel;
        public bool                               isServer;

        public void Execute(
            in                   Entity                 holder
          , in                   NeedSpawnActorDetector spawnData
          , [EntityIndexInQuery] int                    queryId) {
            // Spawn detector
            var detector = ecbParallel.Instantiate(queryId, spawnData.prefab);

            // Set holder
            ecbParallel.SetComponent<ActorDetector>(queryId, detector, holder);

            // Bind detector's life to its holder
            if (isServer) // Only bind in server is enough (the real reason is we cannot do it on the client 🥲)
                ecbParallel.AppendToBuffer<LinkedEntityGroup>(queryId, holder, detector);
        }
    }

    /// <summary>
    /// Use another job to mark the request as completed immediately
    /// </summary>
    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct MarkRequestCompletedJob : IJobEntity {
        public void Execute(EnabledRefRW<NeedSpawnActorDetector> spawnRequest) {
            spawnRequest.ValueRW = false;
        }
    }
}