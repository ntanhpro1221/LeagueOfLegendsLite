using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct MinionSpawnServerSystem : ISystem {
    [ReadOnly] private ComponentLookup<DeadState> deadLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PrefabIdData>();
        state.RequireForUpdate<MinionWaveData>();
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<MinionPrefabBuffer>();

        deadLookup = SystemAPI.GetComponentLookup<DeadState>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        deadLookup.Update(ref state);

        var netTime = SystemAPI.GetSingleton<NetworkTime>();

        state.Dependency = new Job {
            ecbParallel = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter()
          , deadLookup                     = deadLookup
          , prefabs                        = SystemAPI.GetSingletonBuffer<MinionPrefabBuffer>()
          , waveData                       = SystemAPI.GetSingleton<MinionWaveData>()
          , minionId                       = SystemAPI.GetSingleton<PrefabIdData>()._MinionIdRef
          , curTick                        = netTime.ServerTick
          , IsFirstTimeFullyPredictingTick = netTime.IsFirstTimeFullyPredictingTick
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecbParallel;

        [ReadOnly] public ComponentLookup<DeadState>        deadLookup;
        [ReadOnly] public DynamicBuffer<MinionPrefabBuffer> prefabs;

        public MinionWaveData                                                                      waveData;
        public BlobAssetReference<BlobMap<EqualEnum<TeamType>, BlobMap<EqualEnum<MinionId>, int>>> minionId;
        public NetworkTick                                                                         curTick;
        public bool                                                                                IsFirstTimeFullyPredictingTick;

        [BurstCompile]
        public void Execute(
            ref                  MinionSpawnerData                   spawnerData
          , ref                  DynamicBuffer<MinionSpawnQueueData> spawnQueue
          , in                   LaneTypeData                        laneData
          , in                   TeamTypeData                        teamData
          , [EntityIndexInQuery] int                                 queryId) {
            PushWave(ref spawnerData, ref spawnQueue);
            PopWave(ref spawnQueue, laneData, teamData, queryId);
        }

        [BurstCompile]
        private void PushWave(
            ref MinionSpawnerData                   spawnerData
          , ref DynamicBuffer<MinionSpawnQueueData> spawnQueue) {
            while (curTick.IsNewerThan(spawnerData.nextWaveTick)) {
                ref var waveMinions = ref
                    deadLookup.HasComponent(spawnerData.targetInhibitor)
                 && deadLookup.IsComponentEnabled(spawnerData.targetInhibitor)
                        ? ref waveData.waveSuper
                        : ref waveData.waveLoop.Value[spawnerData.curWaveId];
                var spawnTick = curTick;
                for (int i = 0; i < waveMinions.Count; ++i) {
                    spawnQueue.Add(new MinionSpawnQueueData {
                        minionId  = waveMinions[i]
                      , spawnTick = spawnTick
                    });

                    spawnTick.Add(spawnerData.minionInterval);
                }

                // calc next wave info
                spawnerData.ToNextWave(waveData.waveLoop.Value.Count);
            }
        }

        [BurstCompile]
        private void PopWave(
            ref DynamicBuffer<MinionSpawnQueueData> spawnQueue
          , in  LaneTypeData                        laneData
          , in  TeamTypeData                        teamData
          , int                                     queryId) {
            while (!spawnQueue.IsEmpty && curTick.IsNewerThan(spawnQueue[0].spawnTick)) {
                if (IsFirstTimeFullyPredictingTick) {
                    var newMinion = ecbParallel.Instantiate(queryId
                      , prefabs[minionId.Value[teamData.team][spawnQueue[0].minionId]].Entity);

                    ecbParallel.SetComponent(queryId, newMinion, laneData);
                }

                spawnQueue.RemoveAt(0);
            }
        }
    }
}