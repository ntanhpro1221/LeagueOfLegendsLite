using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(UpdateObstacleSystemGroup))]
[UpdateAfter(typeof(RequestObstacleSystem))]
public partial struct ProvideObstacleSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ObstacleConfigData>(); // void run in wrong scene
    }

    public void OnUpdate(ref SystemState state) {
        var provider = ObstacleProvider.Instance;

        // phase 1: Get cutter but avoid create new instance (assign null instead)
        foreach (var (
                obstacle
              , entity)
            in SystemAPI
                .Query<RefRW<ActiveObstacle>>()
                .WithAll<Simulate>()
                .WithEntityAccess())
            obstacle.ValueRW.Obstacle = provider.Get(entity, false);
        
        // release unused cutter
        provider.ReleaseUnusedCutter();
        
        // phase 2: Only get cutter for ActiveObstacle that null in phase 1
        foreach (var (
                obstacle
              , entity)
            in SystemAPI
                .Query<RefRW<ActiveObstacle>>()
                .WithAll<Simulate>()
                .WithEntityAccess())
            if (obstacle.ValueRW.Obstacle == null)
                obstacle.ValueRW.Obstacle = provider.Get(entity, true);
        
        provider.SwapUsedContainer();
    }
}