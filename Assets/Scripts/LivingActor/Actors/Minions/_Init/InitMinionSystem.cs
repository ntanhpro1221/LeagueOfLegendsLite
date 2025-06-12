using NGDtuanh.BubleAsset.ShortCut;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct InitMinionSystem : ISystem {
    private EntityQuery mainQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllMinionData>();
        state.RequireForUpdate<InitTransformData>();

        mainQuery = SystemAPI.QueryBuilder()
            .WithAll<
                MinionTag
              , Simulate
              , NeedInitTag
            >().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (mainQuery.IsEmpty) return;

        state.Dependency = new Job {
            allMinion = SystemAPI.GetSingleton<AllMinionData>()
          , initTrans = SystemAPI.GetSingleton<InitTransformData>()._MinionRef
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(MinionTag)
      , typeof(NeedInitTag))]
    [WithPresent(
        typeof(BountyData)
      , typeof(StatsData_Raw))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public AllMinionData allMinion;

        public BlobAssetReference<Buble_EnMap_EnMap_Array<LaneType, TeamType, InitTransform, Transform>> initTrans;

        [BurstCompile]
        public void Execute(
            // Identity
            in MinionTag    tag
          , in TeamTypeData team
          , in LaneTypeData lane

            // Bounty
          , ref BountyData           bounties
          , EnabledRefRW<BountyData> bountyTrigger

            // Raw stats
          , ref StatsData_Raw           statsRaw
          , EnabledRefRW<StatsData_Raw> statsRawTrigger

            // Position
          , ref LocalTransform locTrans
          , ref RotationData   rotation

            // Control factor
          , ref MinionControlFactor controlFactor

            // Fixed Path
          , ref DynamicBuffer<MinionFixedPathBuffer> pathBuffer) {

            // CACHE
            ref var actor      = ref allMinion.Minions[tag.id];
            ref var pathSource = ref initTrans.Value.Value[lane.laneType][team.team];

            // BOUNTY
            InitHelpers.Bounty(ref bounties, ref bountyTrigger, ref actor.bounty);

            // RAW STATS
            InitHelpers.StatsRaw(ref statsRaw, ref statsRawTrigger
              , source: ref actor.stats);

            // POSITION
            rotation.RotateTo((
                locTrans = pathSource[0].ToLocTrans_Directly()
            ).Forward().Quantizate3().xz);

            // CONTROL FACTOR
            controlFactor.aggroRangeSqr = actor.aggroRange.Sqr();

            // FIXED PATH
            pathBuffer.Resize(pathSource.Count, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < pathBuffer.Length; i++)
                pathBuffer[i] = new MinionFixedPathBuffer { pos = pathSource[i].position };
        }
    }
}