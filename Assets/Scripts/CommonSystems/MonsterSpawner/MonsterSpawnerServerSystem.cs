using NGDtuanh.BubleAsset;
using NGDtuanh.BubleAsset.ShortCut;
using NGDtuanh.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(Between_CopyCommand_PredictedFixed_SystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct MonsterSpawnerServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InitTransformData>();
        state.RequireForUpdate<PrefabIdData>();
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            Simulate
          , MonsterSpawnerData>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var netTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!netTime.IsFirstTimeFullyPredictingTick) return;

        state.Dependency = new Job {
            ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter()
          , curTick = netTime.ServerTick
          , monsterPrefabs = SystemAPI.GetSingletonBuffer<MonsterPrefabBuffer>(
                isReadOnly: true)
          , monsterInitTrans = SystemAPI.GetSingleton<InitTransformData>()._MonsterRef
          , monsterId        = SystemAPI.GetSingleton<PrefabIdData>()._MonsterIdRef
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;
        public NetworkTick                        curTick;

        [ReadOnly] public DynamicBuffer<MonsterPrefabBuffer> monsterPrefabs;

        public BlobAssetReference<Buble_EnMap_EnMap_Array<MonsterId, TeamType, InitTransform, Transform>> monsterInitTrans;
        public BlobAssetReference<BlobMap<EqualEnum<MonsterId>, int>>                                     monsterId;

        [BurstCompile]
        public void Execute(
            in  JungleTeamTypeData                           teamData
          , in  MonsterTag                                   tag
          , in  DynamicBuffer<MonsterExtraBuffer>            extraBuffer
          , in  DynamicBuffer<FollowerEntityFixedPathBuffer> fixedPathBuffer
          , ref MonsterSpawnerData                           spawnData
          , EnabledRefRW<MonsterSpawnerData>                 spawnTrigger
          , [EntityIndexInQuery] int                         queryId) {
            if (spawnData.spawnTick.IsNewerThan(curTick)) return;

            // Mark complete spawn
            spawnTrigger.ValueRW = false;

            // Spawn
            var monster = ecb.Instantiate(queryId, monsterPrefabs[monsterId.Value[tag.id]].Entity);

            // Set team
            ecb.SetComponent(queryId, monster, teamData);

            // Set actor detect init pos
            ecb.SetComponent(queryId, monster, new ActorDetectInitPos {
                pos = monsterInitTrans.Value.Value[tag.id][teamData.team][0].position
            });

            // Set can respawn
            ecb.SetComponentEnabled<MonsterCanRespawn>(queryId, monster, true);

            // VARIANT: Set extra monster
            if (!extraBuffer.IsEmpty) {
                foreach (var extraItem in extraBuffer)
                    ecb.AppendToBuffer(queryId, monster, extraItem);

                ecb.AddComponent(queryId, monster, new MonsterExtraBufferCount { Count = extraBuffer.Length });

                ecb.SetComponent(queryId, monster, new MonsterLeaderData { underlingCount = extraBuffer.Length });
            }

            // VARIANT: Set fixed path
            if (!fixedPathBuffer.IsEmpty)
                foreach (var point in fixedPathBuffer)
                    ecb.AppendToBuffer(queryId, monster, point);
        }
    }
}