using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(InitBattleSystemGroup))]
[UpdateAfter(typeof(HandleInGameRequestServerSystem))]
public partial struct InitChampionServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InitTransformData>();
        state.RequireForUpdate<EnumIndexData>();
        state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp)
            .WithAll<
                ChampionTag
              , Simulate
              , NeedInitTag>()
            .Build(ref state));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        ref var statsEnumIndex     = ref SystemAPI.GetSingleton<EnumIndexData>().StatsType;
        ref var champInitTransform = ref SystemAPI.GetSingleton<InitTransformData>().Champion.Value;

        foreach (var (
                teamType
              , stats
              , health
              , mana
              , localTrans
              , moveData
              , entity)
            in SystemAPI
                .Query<
                    RefRO<TeamTypeData>
                  , DynamicBuffer<StatsBuffer>
                  , RefRW<HealthData>
                  , RefRW<ManaData>
                  , RefRW<LocalTransform>
                  , RefRW<MoveData>>()
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
            localTrans.ValueRW              = champInitTransform[teamType.ValueRO.teamType][0].ToLocalTransform_Directly();
            moveData.ValueRW.targetLocalPos = localTrans.ValueRO.Position.Quantizate3();
        }

        ecb.Playback(state.EntityManager);
    }
}