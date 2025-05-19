using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct InitMinionSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllMinionData>();
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
            initTrans     = SystemAPI.GetSingleton<InitTransformData>()
          , healthId      = statsId[StatsType.Health]
          , allMinionData = SystemAPI.GetSingleton<AllMinionData>()
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
        public AllMinionData     allMinionData;
        public int               healthId;

        [BurstCompile]
        public void Execute(
            in  LaneTypeData                         laneType
          , in  TeamTypeData                         teamType
          , in  MinionTag                            tag
          , in  DynamicBuffer<StatsBuffer>           stats
          , ref HealthData                           health
          , ref LocalTransform                       locTrans
          , ref DynamicBuffer<MinionFixedPathBuffer> pathBuffer
          , ref MinionControlFactor                  controlFactor
          , EnabledRefRW<NeedInitTag>                needInit
          , EnabledRefRW<HealthData>                 healthEnabled) {

            // remove init request
            needInit.ValueRW = false;

            // init health, enable it
            health.value          = stats[healthId].value;
            healthEnabled.ValueRW = true;

            ref var pathSource = ref initTrans.Minion.Value[laneType.laneType][teamType.team];

            // init position
            locTrans = pathSource[0].ToLocTrans_Directly();

            // init path
            pathBuffer.Resize(pathSource.Count, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < pathBuffer.Length; i++)
                pathBuffer[i] = new MinionFixedPathBuffer { pos = pathSource[i].position };

            // init control factor
            controlFactor.aggroRangeSqr = allMinionData.Minions[tag.id].aggroRange.Sqr();
        }
    }
}