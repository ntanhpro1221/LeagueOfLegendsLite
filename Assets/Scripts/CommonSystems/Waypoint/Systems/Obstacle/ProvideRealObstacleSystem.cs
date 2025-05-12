using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(UpdateObstacleSystemGroup))]
[UpdateAfter(typeof(PrepareObstacleDataSystem))]
public partial struct ProvideRealObstacleSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ObstacleConfigData>(); // void run in wrong scene
    }

    public void OnUpdate(ref SystemState state) {
        var provider = ObstacleProvider.Instance;

        provider.ReleaseAllCutter();

        // Not only simulating entity, we must update for all of them
        foreach (var obstacle in SystemAPI.Query<RefRW<ActiveObstacle>>())
            obstacle.ValueRW.Obstacle = provider.Get();
    }
}