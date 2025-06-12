using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(ActorAIControlSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct TurretControlSystem : ISystem {
    [ReadOnly] private ComponentLookup<Selectable>     selectLookup;
    [ReadOnly] private ComponentLookup<LocalTransform> locTransLookup;
    [ReadOnly] private ComponentLookup<TeamTypeData>   teamLookup;
    [ReadOnly] private ComponentLookup<StatsData>      statsLookup;

    private EntityQuery champQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        selectLookup = SystemAPI.GetComponentLookup<Selectable>(
            isReadOnly: true);
        locTransLookup = SystemAPI.GetComponentLookup<LocalTransform>(
            isReadOnly: true);
        teamLookup = SystemAPI.GetComponentLookup<TeamTypeData>(
            isReadOnly: true);
        statsLookup = SystemAPI.GetComponentLookup<StatsData>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        selectLookup.Update(ref state);
        locTransLookup.Update(ref state);
        teamLookup.Update(ref state);
        statsLookup.Update(ref state);

        state.Dependency = new Job {
            selectLookup   = selectLookup
          , locTransLookup = locTransLookup
          , teamLookup     = teamLookup
          , statsLookup    = statsLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(TurretTag))]
    [WithDisabled(
        typeof(DeadState))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [ReadOnly] public ComponentLookup<Selectable>     selectLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> locTransLookup;
        [ReadOnly] public ComponentLookup<TeamTypeData>   teamLookup;
        [ReadOnly] public ComponentLookup<StatsData>      statsLookup;

        [BurstCompile]
        public void Execute(
            ref AimedTargetData                       targetData
          , in  StatsData                             stats
          , in  LocalTransform                        locTrans
          , in  TeamTypeData                          team
          , in  DynamicBuffer<DetectedChampionBuffer> detectedChamp
          , in  DynamicBuffer<DetectedMinionBuffer>   detectedMinion
          , in  AllyBeAttackedData                    allyBeAttacked) {
            // ally champ be attacked by enemy champ
            if (GameHelpers.IsTargetExists(allyBeAttacked.champByChamp, selectLookup)
             && detectedChamp.Contains(allyBeAttacked.champByChamp))
                targetData.target = allyBeAttacked.champByChamp;
            // otherwise seek for target normally
            else {
                if (GameHelpers.IsTargetExists(
                        targetData.target
                      , selectLookup)
                 && !GameHelpers.IsTargetOutOfRange(
                        locTransLookup[targetData.target].Position
                      , locTrans.Position
                      , stats.data.AttackRange
                      , statsLookup[targetData.target].data.UnitRadius))
                    return;

                targetData.target = Entity.Null;
                if (!detectedMinion.IsEmpty) targetData.target     = detectedMinion[0].entity;
                else if (!detectedChamp.IsEmpty) targetData.target = detectedChamp[0].entity;
            }
        }
    }
}