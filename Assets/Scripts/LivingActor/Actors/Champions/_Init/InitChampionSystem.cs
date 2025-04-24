using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct InitChampionSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InitTransformData>();
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAll<
                ChampionTag
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
          , manaId    = statsId[StatsType.Mana]
        }.ScheduleParallel(state.Dependency);
    }

    [WithAll(
        typeof(Simulate)
      , typeof(ChampionTag)
      , typeof(NeedInitTag))]
    [WithPresent(
        typeof(HealthData)
      , typeof(ManaData))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        public InitTransformData initTrans;
        public int               healthId;
        public int               manaId;

        [BurstCompile]
        public void Execute(
            in  TeamTypeData               teamType
          , in  DynamicBuffer<StatsBuffer> stats
          , ref HealthData                 health
          , ref ManaData                   mana
          , ref LocalTransform             locTrans
          , MoveRequesterAspect            moveRequester
          , EnabledRefRW<NeedInitTag>      needInit
          , EnabledRefRW<HealthData>       healthEnabled
          , EnabledRefRW<ManaData>         manaEnabled) {
            // remove init request
            needInit.ValueRW = false;

            // init health, enable it
            health.value          = stats[healthId].value;
            healthEnabled.ValueRW = true;

            // init mana, enable it
            mana.value          = stats[manaId].value;
            manaEnabled.ValueRW = true;

            // init position, move target
            locTrans = initTrans.Champion.Value[teamType.teamType]
                [0]
                .ToLocTrans_Directly();
            moveRequester.SyncFromLocTrans(locTrans);
        }
    }
}