using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct CachedPredictWaypointCleanupSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAny<
                CachedPathData
              , CachedLineCastData
              , HandlingPathData>()
            .WithNone<LocalTransform>()
            .Build());
    }

    public void TryCleanup_PathData(ref SystemState state) {
        if (!SystemAPI.ManagedAPI.TryGetSingletonEntity<CachedPathData>(out var entity)) return;
        SystemAPI.ManagedAPI.GetComponent<CachedPathData>(entity).Dispose();
        state.EntityManager.RemoveComponent<CachedPathData>(entity);
    }
    
    public void TryCleanup_LineCastData(ref SystemState state) {
        if (!SystemAPI.ManagedAPI.TryGetSingletonEntity<CachedLineCastData>(out var entity)) return;
        SystemAPI.ManagedAPI.GetComponent<CachedLineCastData>(entity).Dispose();
        state.EntityManager.RemoveComponent<CachedLineCastData>(entity);
    }

    public void TryCleanup_HandlingData(ref SystemState state) {
        if (!SystemAPI.ManagedAPI.TryGetSingletonEntity<HandlingPathData>(out var entity)) return;
        SystemAPI.ManagedAPI.GetComponent<HandlingPathData>(entity).Dispose();
        state.EntityManager.RemoveComponent<HandlingPathData>(entity);
    }
    
    public void OnUpdate(ref SystemState state) {
        TryCleanup_PathData(ref state);
        TryCleanup_LineCastData(ref state);
        TryCleanup_HandlingData(ref state);
    }

    public void OnDestroy(ref SystemState state) { 
        TryCleanup_PathData(ref state);
        TryCleanup_LineCastData(ref state);
        TryCleanup_HandlingData(ref state);
    }
}