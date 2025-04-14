using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Just auto target to champion now
/// </summary>
[UpdateInGroup(typeof(ActorAIControlSystemGroup))]
public partial struct TurretControlSystem : ISystem {
    [ReadOnly] private EntityStorageInfoLookup         entityLookup;
    [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
    [ReadOnly] private ComponentLookup<TeamTypeData>   teamLookup;
    [ReadOnly] private BufferLookup<StatsBuffer>       statsLookup;

    private EntityQuery champQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();

        entityLookup = SystemAPI.GetEntityStorageInfoLookup();
        selectLookup = SystemAPI.GetComponentLookup<Selectable>(
            isReadOnly: true);
        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
        teamLookup = SystemAPI.GetComponentLookup<TeamTypeData>(
            isReadOnly: true);
        statsLookup = SystemAPI.GetBufferLookup<StatsBuffer>(
            isReadOnly: true);

        champQuery = SystemAPI.QueryBuilder()
            .WithAll<
                ChampionTag
              , Selectable>()
            .Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ref var stateId = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;

        entityLookup.Update(ref state);
        selectLookup.Update(ref state);
        locTransLookup.Update(ref state);
        teamLookup.Update(ref state);
        statsLookup.Update(ref state);

        state.Dependency = new Job {
            entityLookup   = entityLookup
          , selectLookup   = selectLookup
          , locTransLookup = locTransLookup
          , teamLookup     = teamLookup
          , statsLookup    = statsLookup

          , champEntity = champQuery.ToEntityArray(Allocator.TempJob)

          , attackRangeId = stateId[StatsType.AttackRange]
          , unitRadiusId  = stateId[StatsType.UnitRadius]
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(TurretTag))]
    [WithDisabled(
        typeof(DeadState))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [ReadOnly] public EntityStorageInfoLookup         entityLookup;
        [ReadOnly] public ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] public ComponentLookup<TeamTypeData>   teamLookup;
        [ReadOnly] public BufferLookup<StatsBuffer>       statsLookup;

        [DeallocateOnJobCompletion, ReadOnly] public NativeArray<Entity> champEntity;

        public int attackRangeId;
        public int unitRadiusId;

        [BurstCompile]
        public void Execute(
            ref AimedTargetData            targetData
          , in  Entity                     entity
          , in  DynamicBuffer<StatsBuffer> stats
          , in  LocalTransform             locTrans
          , in  TeamTypeData               team) {
            if (GameHelpers.IsTargetExists(
                    targetData.target
                  , entityLookup
                  , selectLookup)
             && !GameHelpers.IsTargetOutOfRange(
                    locTransLookup[targetData.target]
                  , locTrans
                  , stats[attackRangeId].value
                  , statsLookup[targetData.target][unitRadiusId].value))
                return;
            targetData.target = Entity.Null;

            foreach (var target in champEntity) {
                // Not now
                // if (champTeam[i].IsSameTeam(team)) continue;

                if (GameHelpers.IsTargetOutOfRange(
                    locTransLookup[target]
                  , locTrans
                  , stats[attackRangeId].value
                  , statsLookup[target][unitRadiusId].value))
                    continue;

                targetData.target = target;
                return;
            }
        }
    }
}