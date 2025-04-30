using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ActorGeneralInitSystemGroup))]
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
          , side      = state.WorldName()
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
        public InitTransformData  initTrans;
        public int                healthId;
        public FixedString32Bytes side;

        [BurstCompile]
        public void Execute(
            in  LaneTypeData                         laneType
          , in  TeamTypeData                         teamType
          , in  DynamicBuffer<StatsBuffer>           stats
          , ref HealthData                           health
          , ref LocalTransform                       locTrans
          , ref DynamicBuffer<MinionFixedPathBuffer> pathBuffer
          , MoveRequesterAspect                      moveRequester
          , EnabledRefRW<NeedInitTag>                needInit
          , EnabledRefRW<HealthData>                 healthEnabled) {

            // remove init request
            needInit.ValueRW = false;

            // init health, enable it
            health.value          = stats[healthId].value;
            healthEnabled.ValueRW = true;

            ref var pathSource = ref initTrans.Minion.Value[laneType.laneType][teamType.teamType];

            // init position
            locTrans = pathSource[0].ToLocTrans_Directly();
            moveRequester.SyncFromLocTrans(locTrans);

            // init path
            pathBuffer.Resize(pathSource.Count, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < pathBuffer.Length; i++)
                pathBuffer[i] = new MinionFixedPathBuffer { pos = pathSource[i].position };
        }
    }
}