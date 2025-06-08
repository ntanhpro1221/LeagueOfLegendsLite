using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct SpawnExtraMonsterServerSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<MonsterPrefabBuffer>();
        state.RequireForUpdate<PrefabIdData>();
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                Simulate
              , MonsterExtraTrigger
            >().WithDisabled<
                NeedInitTag
            >().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        state.Dependency = new Job {
            ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter()
          , monsterId = SystemAPI.GetSingleton<PrefabIdData>()._MonsterIdRef
          , monsterPrefabs = SystemAPI.GetSingletonBuffer<MonsterPrefabBuffer>(
                isReadOnly: true)
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [WithDisabled(typeof(NeedInitTag))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public EntityCommandBuffer.ParallelWriter                     ecb;
        public BlobAssetReference<BlobMap<EqualEnum<MonsterId>, int>> monsterId;

        [ReadOnly] public DynamicBuffer<MonsterPrefabBuffer> monsterPrefabs;

        [BurstCompile]
        public void Execute(
            in DynamicBuffer<MonsterExtraBuffer> spawnBuffer
          , in JungleTeamTypeData                teamType
          , in Entity                            entity
          , EnabledRefRW<MonsterExtraTrigger>    spawnTrigger
          , [EntityIndexInQuery] int             queryId) {
            // Mark spawn complete
            spawnTrigger.ValueRW = false;

            foreach (var spawnItem in spawnBuffer) {
                // spawn
                var monster = ecb.Instantiate(queryId, monsterPrefabs[monsterId.Value[spawnItem.id]]);

                // set init transform
                var monsterLocTrans = spawnItem.initTrans.ToLocTrans_Directly();
                ecb.SetComponent(queryId, monster, monsterLocTrans);
                ecb.SetComponent(queryId, monster, MonsterLeashAnchor.FromLocTrans(monsterLocTrans));

                // set jungle team
                ecb.SetComponent(queryId, monster, teamType);

                // set leader link
                ecb.SetComponent(queryId, monster, new MonsterUnderlingData { leader = entity });

                // set underling link
                ecb.AppendToBuffer(queryId, entity, new MonsterMyUnderlingBuffer { entity = monster });
            }
        }
    }
}