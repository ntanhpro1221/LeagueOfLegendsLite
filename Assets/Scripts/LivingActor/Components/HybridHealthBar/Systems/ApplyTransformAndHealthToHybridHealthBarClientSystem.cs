using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct ApplyTransformAndHealthToHybridHealthBarClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnumIndexData>();
        using EntityQueryBuilder queryBuilder = new(Allocator.Temp);
        state.RequireForUpdate(queryBuilder
            .WithAll<
                HybridHealthBarData
              , StatsData
              , HealthData
              , ManaData
              , LevelData
              , LocalToWorld>()
            .Build(ref state));
    }

    public void OnUpdate(ref SystemState state) {
        using EntityCommandBuffer ecb = new(Allocator.Temp);

        var     cam            = Camera.main;
        ref var statsEnumIndex = ref SystemAPI.GetSingleton<EnumIndexData>().ChampionStatsType;

        foreach (var (
                hybridData
              , statsData
              , healthData
              , manaData
              , levelData
              , localToWorld)
            in SystemAPI.Query<
                RefRO<HybridHealthBarData>
              , DynamicBuffer<StatsData>
              , RefRO<HealthData>
              , RefRO<ManaData>
              , RefRO<LevelData>
              , RefRO<LocalToWorld>>()) {
            var worldPos = localToWorld.ValueRO.Position;
            worldPos.y += hybridData.ValueRO.deltaY;
            hybridData.ValueRO.transRef.Value.position =  cam!.WorldToScreenPoint(worldPos);

            hybridData.ValueRO.UIRef.Value.UpdateUI(
                maxHealth: statsData[statsEnumIndex[ChampionStatsType.Health]].FullValue
              , curHealth: healthData.ValueRO.value
              , curArmor: 0
              , maxMana: statsData[statsEnumIndex[ChampionStatsType.Mana]].FullValue
              , curMana: manaData.ValueRO.value
              , curLevel: levelData.ValueRO.curLevel);
        }

        ecb.Playback(state.EntityManager);
    }
}