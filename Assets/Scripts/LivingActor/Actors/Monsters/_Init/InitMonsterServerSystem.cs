using NGDtuanh.BubleAsset.ShortCut;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct InitMonsterServerSystem : ISystem {
    private EntityQuery mainQuery;

    [ReadOnly] private ComponentLookup<MonsterManualInitTransAndAnchorTag> manualLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllMonsterData>();
        state.RequireForUpdate<InitTransformData>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                MonsterTag
              , Simulate
              , NeedInitTag
            >().Build();

        manualLookup = SystemAPI.GetComponentLookup<MonsterManualInitTransAndAnchorTag>(
            isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        manualLookup.Update(ref state);
        state.Dependency = new Job {
            allMonster   = SystemAPI.GetSingleton<AllMonsterData>()
          , initTrans    = SystemAPI.GetSingleton<InitTransformData>()._MonsterRef
          , manualLookup = manualLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MonsterTag)
      , typeof(NeedInitTag))]
    [WithPresent(
        typeof(BountyData)
      , typeof(StatsData_Raw)
      , typeof(MonsterLeashAnchor))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public AllMonsterData allMonster;

        public BlobAssetReference<Buble_EnMap_EnMap_Array<MonsterId, TeamType, InitTransform, Transform>> initTrans;

        [ReadOnly] public ComponentLookup<MonsterManualInitTransAndAnchorTag> manualLookup;

        [BurstCompile]
        public void Execute(
            // Identity
            in MonsterTag         tag
          , in JungleTeamTypeData teamJungle
          , in Entity             entity

            // Bounty
          , ref BountyData           bounties
          , EnabledRefRW<BountyData> bountyTrigger

            // Raw stats
          , ref StatsData_Raw           statsRaw
          , EnabledRefRW<StatsData_Raw> statsRawTrigger

            // Position
          , ref LocalTransform     locTrans
          , ref RotationData       rotation
          , ref MonsterLeashAnchor anchorData

            // Control Factor
          , ref MonsterControlFactor controlFactor) {
            // CACHE
            ref var actor = ref allMonster.Monsters[tag.id];

            // BOUNTY
            InitHelpers.Bounty(ref bounties, ref bountyTrigger, ref actor.bounty);

            // RAW STATS
            InitHelpers.StatsRaw(ref statsRaw, ref statsRawTrigger
              , source: ref actor.stats);

            // POSITION
            if (!manualLookup.HasComponent(entity)) {
                locTrans = initTrans.Value.Value[tag.id][teamJungle.team][0].ToLocTrans_Directly();
                rotation.RotateTo(locTrans.Forward().Quantizate3().xz);
                anchorData = MonsterLeashAnchor.FromLocTrans(locTrans);
            }

            // CONTROL FACTOR
            controlFactor.leashRangeSqr = actor.leashRange.Sqr();
            controlFactor.respawnCDTick = actor.respawnCDTick;
        }

    }
}