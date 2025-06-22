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
            initTrans = SystemAPI.GetSingleton<InitTransformData>()._TowerRef
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(TowerTag)
      , typeof(NeedInitTag))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public BlobAssetReference<Buble_EnMap_EnMap_EnMap<TeamType, TowerId, LaneType, InitTransform, Transform>> initTrans;

        [BurstCompile]
        public void Execute(
            // Identity
            in TowerTag     tag
          , in TeamTypeData team
          , in LaneTypeData lane

            // Position
          , ref LocalTransform locTrans
          , ref RotationData   rotation) {

            // POSITION
            rotation.RotateTo((
                locTrans = initTrans.Value.Value[team.team][tag.id][lane.laneType].ToLocTrans_Directly()
            ).Forward().Quantizate3().xz);
        }
    }
} 