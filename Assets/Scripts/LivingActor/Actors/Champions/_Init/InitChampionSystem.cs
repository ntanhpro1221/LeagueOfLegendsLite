using NGDtuanh.BubleAsset.ShortCut;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
public partial struct InitChampionSystem : ISystem {
    private EntityQuery mainQuery;

    [ReadOnly] private ComponentLookup<DummyTag> dummyLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllChampionData>();
        state.RequireForUpdate<InitTransformData>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                ChampionTag
              , Simulate
              , NeedInitTag
            >().Build();

        dummyLookup = SystemAPI.GetComponentLookup<DummyTag>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        dummyLookup.Update(ref state);
        state.Dependency = new Job {
            allChamp    = SystemAPI.GetSingleton<AllChampionData>()
          , initTrans   = SystemAPI.GetSingleton<InitTransformData>()._ChampionRef
          , dummyLookup = dummyLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(ChampionTag)
      , typeof(NeedInitTag))]
    [WithPresent(
        typeof(BountyData)
      , typeof(StatsData_Raw)
      , typeof(StatsData_RawPerLevel))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public AllChampionData allChamp;

        public BlobAssetReference<Buble_EnMap_Array<TeamType, InitTransform, Transform>> initTrans;

        [ReadOnly] public ComponentLookup<DummyTag> dummyLookup;

        [BurstCompile]
        public void Execute(
            // Identity
            in TeamTypeData team
          , in ChampionTag  tag
          , in Entity       entity

            // Bounty
          , ref BountyData           bounties
          , EnabledRefRW<BountyData> bountyTrigger

            // Raw stats
          , ref StatsData_Raw                   statsRaw
          , ref StatsData_RawPerLevel           statsRawPerLevel
          , EnabledRefRW<StatsData_Raw>         statsRawTrigger
          , EnabledRefRW<StatsData_RawPerLevel> statsRawPerLevelTrigger

            // Position
          , ref LocalTransform  locTrans
          , MoveRequesterAspect moveRequester) {

            // CACHE
            ref var actor = ref allChamp.Champions[tag.id];

            // BOUNTY
            InitHelpers.Bounty(ref bounties, ref bountyTrigger, ref allChamp.CommonInitBounty);

            // RAW STATS
            InitHelpers.StatsRaw(ref statsRaw, ref statsRawPerLevel, ref statsRawTrigger, ref statsRawPerLevelTrigger
              , source: ref actor.stats
              , sourcePerLevel: ref actor.statsPerLevel);

            // POSITION (not init for dummy)
            if (!dummyLookup.HasComponent(entity))
                locTrans = initTrans.Value.Value[team.team][0].ToLocTrans_Directly();
            moveRequester.SyncFromLocTrans(locTrans);
        }
    }
}