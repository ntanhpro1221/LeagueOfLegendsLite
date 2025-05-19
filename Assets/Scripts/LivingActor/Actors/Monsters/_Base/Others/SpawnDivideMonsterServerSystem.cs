using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct SpawnDivideMonsterServerSystem : ISystem {
    [ReadOnly] private ComponentLookup<Selectable> selectLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<MonsterPrefabBuffer>();
        state.RequireForUpdate<PrefabIdData>();
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<
            Simulate
          , MonsterDivideTrigger>().Build());

        selectLookup = SystemAPI.GetComponentLookup<Selectable>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (!SystemAPI.GetSingleton<NetworkTime>().IsFirstTimeFullyPredictingTick) return;

        selectLookup.Update(ref state);

        state.Dependency = new Job {
            selectLookup = selectLookup
          , ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter()
          , radiusId = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.UnitRadius]
          , monsterPrefabs = SystemAPI.GetSingletonBuffer<MonsterPrefabBuffer>(
                isReadOnly: true)
          , monsterId = SystemAPI.GetSingleton<PrefabIdData>()._MonsterIdRef
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [WithPresent(typeof(MonsterLeashAnchor))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [ReadOnly] public ComponentLookup<Selectable> selectLookup;

        public EntityCommandBuffer.ParallelWriter ecb;
        public int                                radiusId;

        [ReadOnly] public DynamicBuffer<MonsterPrefabBuffer>                     monsterPrefabs;
        public            BlobAssetReference<BlobMap<EqualEnum<MonsterId>, int>> monsterId;

        [BurstCompile]
        public void Execute(
            in DynamicBuffer<StatsBuffer>         stats
          , in MonsterLeashAnchor                 anchor
          , in JungleTeamTypeData                 teamType
          , in LocalTransform                     locTrans
          , in AimedTargetData                    targetData
          , in DynamicBuffer<MonsterDivideBuffer> spawnBuffer
          , EnabledRefRW<MonsterDivideTrigger>    spawnTrigger
          , MonsterCampRootRO                     campRoot
          , [EntityIndexInQuery] int              queryId) {
            // Mark spawn complete
            spawnTrigger.ValueRW = false;
            Entity root   = campRoot.RootUnsafe;
            float  radius = stats[radiusId].value;
            float  curRad = 0;
            float  delRad = math.PI2 / spawnBuffer.Length;

            foreach (var spawnItem in spawnBuffer) {
                // spawn
                var monster = ecb.Instantiate(queryId, monsterPrefabs[monsterId.Value[spawnItem.id]]);

                // set init transform
                curRad += delRad;
                ecb.SetComponent(queryId, monster, locTrans);
                ecb.SetComponent(queryId, monster, new MonsterLeashAnchor {
                    anchorPos = anchor.anchorPos
                      + (radius * new float3(math.sin(curRad), 0, math.cos(curRad))).Quantizate3()
                  , anchorDir = anchor.anchorDir
                });

                // because cur position may not at anchor, so this monster is leashing
                ecb.SetComponentEnabled<MonsterLeashAnchor>(queryId, monster, true);

                // set target
                if (GameHelpers.IsTargetExists(targetData.target, selectLookup))
                    ecb.SetComponent(queryId, monster, new AimedTargetData { target = targetData.target });

                // set jungle team
                ecb.SetComponent(queryId, monster, teamType);

                // set leader link
                ecb.SetComponent(queryId, monster, new MonsterUnderlingData { leader = root });

                // set underling link
                ecb.AppendToBuffer(queryId, root, new MonsterMyUnderlingBuffer { entity = monster });
            }
        }
    }
}