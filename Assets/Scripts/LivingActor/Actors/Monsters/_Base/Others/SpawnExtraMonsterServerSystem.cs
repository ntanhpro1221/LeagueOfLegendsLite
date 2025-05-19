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
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<MonsterPrefabBuffer>();
        state.RequireForUpdate<PrefabIdData>();
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            Simulate
          , MonsterExtraTrigger>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        state.Dependency = new Job {
            ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter()
          , monsterPrefabs = SystemAPI.GetSingletonBuffer<MonsterPrefabBuffer>(
                isReadOnly: true)
          , monsterId = SystemAPI.GetSingleton<PrefabIdData>()._MonsterIdRef
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;

        [ReadOnly] public DynamicBuffer<MonsterPrefabBuffer>                     monsterPrefabs;
        public            BlobAssetReference<BlobMap<EqualEnum<MonsterId>, int>> monsterId;

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