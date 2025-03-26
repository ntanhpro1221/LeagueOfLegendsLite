using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateAfter(typeof(HandleInGameRequestServerSystem))]
public partial struct InitChampionServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
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

        ref var statsEnumIndex = ref SystemAPI.GetSingleton<EnumIndexData>().ChampionStatsType;

        foreach (var (
                stats
              , health
              , mana
              , entity)
            in SystemAPI
                .Query<
                    DynamicBuffer<StatsData>
                  , RefRW<HealthData>
                  , RefRW<ManaData>>()
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
            health.ValueRW.value = stats[statsEnumIndex[ChampionStatsType.Health]].FullValue;
            ecb.SetComponentEnabled<HealthData>(entity, true);

            // init mana, enable it
            mana.ValueRW.value = stats[statsEnumIndex[ChampionStatsType.Mana]].FullValue;
            ecb.SetComponentEnabled<ManaData>(entity, true);
        }

        ecb.Playback(state.EntityManager);
    }
}