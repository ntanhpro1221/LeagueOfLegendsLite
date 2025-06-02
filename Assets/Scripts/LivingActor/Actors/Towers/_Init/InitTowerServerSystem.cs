using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct InitTowerServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InitTransformData>();
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                TowerTag
              , Simulate
              , NeedInitTag>()
            .Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ref var statsId = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;

        state.Dependency = new Job {
            initTrans = SystemAPI.GetSingleton<InitTransformData>()
          , healthId  = statsId[StatsType.Health]
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(TowerTag)
      , typeof(NeedInitTag))]
    [WithPresent(
        typeof(HealthData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public InitTransformData initTrans;
        public int               healthId;

        [BurstCompile]
        public void Execute(
            in  TowerTag                   towerTag
          , in  LaneTypeData               laneType
          , in  TeamTypeData               teamType
          , in  DynamicBuffer<StatsBuffer> stats
          , ref HealthData                 health
          , ref LocalTransform             locTrans
          , ref RotationData               rotation
          , EnabledRefRW<NeedInitTag>      needInit
          , EnabledRefRW<HealthData>       healthEnabled) {
            // remove init request
            needInit.ValueRW = false;

            // init health, enable it
            health.value          = stats[healthId].value;
            healthEnabled.ValueRW = true;

            // init position
            rotation.RotateTo((locTrans = initTrans.Tower.Value[teamType.team]
                    [towerTag.id][laneType.laneType].ToLocTrans_Directly())
                .Forward().Quantizate3().xz);
        }
    }
}