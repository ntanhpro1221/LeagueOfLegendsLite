using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
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
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        ref var statsEnumIndex = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;
        ref var towerInitTrans = ref SystemAPI.GetSingleton<InitTransformData>().Tower.Value;

        foreach (var (
                towerTag
              , laneType
              , teamType
              , stats
              , health
              , localTrans
              , entity)
            in SystemAPI
                .Query<
                    RefRO<TowerTag>
                  , RefRO<LaneTypeData>
                  , RefRO<TeamTypeData>
                  , DynamicBuffer<StatsBuffer>
                  , RefRW<HealthData>
                  , RefRW<LocalTransform>>()
                .WithAll<
                    TowerTag
                  , Simulate
                  , NeedInitTag>()
                .WithDisabled<
                    HealthData>()
                .WithEntityAccess()) {
            // remove init request
            ecb.RemoveComponent<NeedInitTag>(entity);

            // init health, enable it
            health.ValueRW.value = stats[statsEnumIndex[StatsType.Health]].value;
            ecb.SetComponentEnabled<HealthData>(entity, true);

            // init position
            localTrans.ValueRW = towerInitTrans[teamType.ValueRO.teamType]
                [towerTag.ValueRO.id]
                [laneType.ValueRO.laneType]
                .ToLocTrans_Directly();
        }

        ecb.Playback(state.EntityManager);
    }
}