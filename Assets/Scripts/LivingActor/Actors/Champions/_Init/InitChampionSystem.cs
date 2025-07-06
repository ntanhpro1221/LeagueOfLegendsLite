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
            initTrans   = SystemAPI.GetSingleton<InitTransformData>()._ChampionRef
          , dummyLookup = dummyLookup
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(ChampionTag)
      , typeof(NeedInitTag))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public BlobAssetReference<Buble_EnMap_Array<TeamType, InitTransform, Transform>> initTrans;

        [ReadOnly] public ComponentLookup<DummyTag> dummyLookup;

        [BurstCompile]
        public void Execute(
            // Identity
            in TeamTypeData team
          , in Entity       entity

            // Position
          , ref LocalTransform      locTrans
          , in  ChampionOrderInTeam orderData
          , MoveRequesterAspect     moveRequester) {

            // POSITION (not init for dummy)
            if (!dummyLookup.HasComponent(entity))
                locTrans = initTrans.Value.Value[team.team][orderData.order].ToLocTrans_Directly();
            moveRequester.SyncFromLocTrans(locTrans);
        }
    }
}