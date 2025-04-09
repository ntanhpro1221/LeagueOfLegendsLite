using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(BeforeInitAndUpdateStatsSystemGroup))]
public partial struct CleanRawStatsRefSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireAnyForUpdate(
            SystemAPI.QueryBuilder()
                .WithAll<RawStatsData>()
                .WithNone<LocalTransform>().Build()
          , SystemAPI.QueryBuilder()
                .WithAll<RawStatsPerLevelData>()
                .WithNone<LocalTransform>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (rawStats, entity) in SystemAPI
            .Query<RefRW<RawStatsData>>()
            .WithNone<LocalTransform>()
            .WithEntityAccess()) {
            rawStats.ValueRW._Ref.Dispose();
            ecb.RemoveComponent<RawStatsData>(entity);
        }

        foreach (var (rawStatsPerLevel, entity) in SystemAPI
            .Query<RefRW<RawStatsPerLevelData>>()
            .WithNone<LocalTransform>()
            .WithEntityAccess()) {
            rawStatsPerLevel.ValueRW._Ref.Dispose();
            ecb.RemoveComponent<RawStatsPerLevelData>(entity);
        }

        ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        using var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (rawStats, entity) in SystemAPI
            .Query<RefRW<RawStatsData>>()
            .WithEntityAccess()) {
            rawStats.ValueRW._Ref.Dispose();
            ecb.RemoveComponent<RawStatsData>(entity);
        }

        foreach (var (rawStatsPerLevel, entity) in SystemAPI
            .Query<RefRW<RawStatsPerLevelData>>()
            .WithEntityAccess()) {
            rawStatsPerLevel.ValueRW._Ref.Dispose();
            ecb.RemoveComponent<RawStatsPerLevelData>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}