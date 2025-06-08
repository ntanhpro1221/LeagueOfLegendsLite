using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct SpawnActorDetectorSystem : ISystem {
    private EntityQuery mainQuery;
    private bool        isServer;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<NetworkTime>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                NeedSpawnActorDetector
              , Simulate
            >().Build();

        isServer = state.WorldUnmanaged.IsServer();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        state.Dependency = new SpawnJob {
            ecbParallel = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter()
          , isServer                       = isServer
          , isFirstTimeFullyPredictingTick = SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct SpawnJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecbParallel;
        public bool                               isServer;
        public bool                               isFirstTimeFullyPredictingTick;

        [BurstCompile]
        public void Execute(
            in  Entity                           holder
          , ref NeedSpawnActorDetector           spawnData
          , in  ActorDetectInitPos               initPos
          , EnabledRefRW<NeedSpawnActorDetector> spawnTrigger
          , [EntityIndexInQuery] int             queryId) {
            // Mark completed
            spawnTrigger.ValueRW = false;

            if (isFirstTimeFullyPredictingTick) {
                // Spawn detector
                var detector = ecbParallel.Instantiate(queryId, spawnData.prefab);

                // Set holder
                ecbParallel.SetComponent<ActorDetector>(queryId, detector, holder);

                // Set init position
                ecbParallel.SetComponent(queryId, detector, LocalTransform.FromPosition(initPos.pos));

                // Bind detector's life to its holder
                if (isServer) // Only bind in server is enough (the real reason is we cannot do it on the client 🥲)
                    ecbParallel.AppendToBuffer<LinkedEntityGroup>(queryId, holder, detector);
            }
        }
    }
}