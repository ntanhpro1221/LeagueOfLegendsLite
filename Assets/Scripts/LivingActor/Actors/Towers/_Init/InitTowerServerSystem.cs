using NGDtuanh.BubleAsset.ShortCut;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct InitTowerServerSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllTowerData>();
        state.RequireForUpdate<InitTransformData>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                TowerTag
              , Simulate
              , NeedInitTag
            >().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        state.Dependency = new Job {
            allTower  = SystemAPI.GetSingleton<AllTowerData>()
          , initTrans = SystemAPI.GetSingleton<InitTransformData>()._TowerRef
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(TowerTag)
      , typeof(NeedInitTag))]
    [WithPresent(
        typeof(BountyBuffer)
      , typeof(StatsBuffer_Raw))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public AllTowerData allTower;

        public BlobAssetReference<Buble_EnMap_EnMap_EnMap<TeamType, TowerId, LaneType, InitTransform, Transform>> initTrans;

        [BurstCompile]
        public void Execute(
            // Identity
            in TowerTag     tag
          , in TeamTypeData team
          , in LaneTypeData lane

            // Bounty
          , ref DynamicBuffer<BountyBuffer> bounties
          , EnabledRefRW<BountyBuffer>      bountyTrigger

            // Raw stats
          , ref DynamicBuffer<StatsBuffer_Raw> statsRaw
          , EnabledRefRW<StatsBuffer_Raw>      statsRawTrigger

            // Position
          , ref LocalTransform locTrans
          , ref RotationData   rotation) {
            // CACHE
            ref var actor = ref allTower.Towers[tag.id];

            // BOUNTY
            InitHelpers.Bounty(ref bounties, ref bountyTrigger, ref actor.bounty);

            // RAW STATS
            InitHelpers.StatsRaw(ref statsRaw, ref statsRawTrigger
              , source: ref actor.stats);

            // POSITION
            rotation.RotateTo((
                locTrans = initTrans.Value.Value[team.team][tag.id][lane.laneType].ToLocTrans_Directly()
            ).Forward().Quantizate3().xz);
        }
    }
}