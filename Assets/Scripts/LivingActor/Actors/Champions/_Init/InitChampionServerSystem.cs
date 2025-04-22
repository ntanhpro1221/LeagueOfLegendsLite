using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct InitChampionServerSystem : ISystem {
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
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        ref var statsEnumIndex = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;
        ref var champInitTrans = ref SystemAPI.GetSingleton<InitTransformData>().Champion.Value;

        foreach (var (
                teamType
              , stats
              , health
              , mana
              , localTrans
              , moveRequester
              , entity)
            in SystemAPI
                .Query<
                    RefRO<TeamTypeData>
                  , DynamicBuffer<StatsBuffer>
                  , RefRW<HealthData>
                  , RefRW<ManaData>
                  , RefRW<LocalTransform>
                  , MoveRequesterAspect>()
                .WithAll<
                    ChampionTag
                  , Simulate
                  , NeedInitTag>()
                .WithDisabled<
                    HealthData
                  , ManaData>()
                .WithEntityAccess()) {
            // remove init request
            ecb.RemoveComponent<NeedInitTag>(entity);

            // init health, enable it
            health.ValueRW.value = stats[statsEnumIndex[StatsType.Health]].value;
            ecb.SetComponentEnabled<HealthData>(entity, true);

            // init mana, enable it
            mana.ValueRW.value = stats[statsEnumIndex[StatsType.Mana]].value;
            ecb.SetComponentEnabled<ManaData>(entity, true);

            // init position, move target
            localTrans.ValueRW = champInitTrans[teamType.ValueRO.teamType][0].ToLocTrans_Directly();
            moveRequester.SyncFromLocTrans(localTrans.ValueRO);
        }

        ecb.Playback(state.EntityManager);
    }
}