using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct InitMinionSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InitTransformData>();
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                MinionTag
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
      , typeof(MinionTag)
      , typeof(NeedInitTag))]
    [WithPresent(
        typeof(HealthData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public InitTransformData initTrans;
        public int               healthId;

        [BurstCompile]
        public void Execute(
            in  LaneTypeData               laneType
          , in  TeamTypeData               teamType
          , in  DynamicBuffer<StatsBuffer> stats
          , ref HealthData                 health
          , ref LocalTransform             locTrans
          , MoveRequesterAspect            moveRequester
          , EnabledRefRW<NeedInitTag>      needInit
          , EnabledRefRW<HealthData>       healthEnabled) {
            // remove init request
            needInit.ValueRW = false;

            // init health, enable it
            health.value          = stats[healthId].value;
            healthEnabled.ValueRW = true;

            // init position
            locTrans = initTrans.Minion.Value[laneType.laneType]
                [teamType.teamType]
                [0]
                .ToLocTrans_Directly();
            moveRequester.SyncFromLocTrans(locTrans);
        }
    }
}