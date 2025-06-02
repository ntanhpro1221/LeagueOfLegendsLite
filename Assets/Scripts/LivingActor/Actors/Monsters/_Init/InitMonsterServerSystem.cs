using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct InitMonsterServerSystem : ISystem {
    private ComponentLookup<MonsterManualInitTransAndAnchorTag> manualTransLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllMonsterData>();
        state.RequireForUpdate<InitTransformData>();
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                MonsterTag
              , Simulate
              , NeedInitTag>()
            .Build());

        manualTransLookup = SystemAPI.GetComponentLookup<MonsterManualInitTransAndAnchorTag>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        manualTransLookup.Update(ref state);
        ref var statsId = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;

        state.Dependency = new Job {
            initTrans         = SystemAPI.GetSingleton<InitTransformData>()
          , healthId          = statsId[StatsType.Health]
          , allMonsterData    = SystemAPI.GetSingleton<AllMonsterData>()
          , manualTransLookup = manualTransLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag)
      , typeof(NeedInitTag))]
    [WithPresent(
        typeof(HealthData)
      , typeof(MonsterLeashAnchor))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public InitTransformData initTrans;
        public AllMonsterData    allMonsterData;
        public int               healthId;

        [ReadOnly] public ComponentLookup<MonsterManualInitTransAndAnchorTag> manualTransLookup;

        [BurstCompile]
        public void Execute(
            in  MonsterTag                 tag
          , in  JungleTeamTypeData         teamType
          , in  DynamicBuffer<StatsBuffer> stats
          , ref MonsterLeashAnchor         anchorData
          , ref HealthData                 health
          , ref LocalTransform             locTrans
          , ref MonsterControlFactor       controlFactor
          , ref RotationData               rotation
          , EnabledRefRW<NeedInitTag>      needInit
          , EnabledRefRW<HealthData>       healthEnabled
          , in Entity                      entity) {

            // remove init request
            needInit.ValueRW = false;

            // init health, enable it
            health.value          = stats[healthId].value;
            healthEnabled.ValueRW = true;

            // init position
            if (!manualTransLookup.HasComponent(entity)) {
                locTrans = initTrans.Monster.Value[tag.id][teamType.team][0].ToLocTrans_Directly();
                rotation.RotateTo(locTrans.Forward().Quantizate3().xz);
                anchorData = MonsterLeashAnchor.FromLocTrans(locTrans);
            }

            // init control factor
            controlFactor.leashRangeSqr = allMonsterData.Monsters[tag.id].leashRange.Sqr();
            controlFactor.respawnCDTick = allMonsterData.Monsters[tag.id].respawnCDTick;
        }
    }
}