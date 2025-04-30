using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(UpdateObstacleSystemGroup))]
[UpdateAfter(typeof(RequestObstacleSystem))]
public partial struct ProvideObstacleSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ObstacleConfigData>(); // void run in wrong scene
    }

    public void OnUpdate(ref SystemState state) {
        var pool = ObstacleProvider.Instance;

        foreach (var (
                obstacle
              , entity)
            in SystemAPI
                .Query<RefRW<ActiveObstacle>>()
                .WithAll<Simulate>()
                .WithEntityAccess())
            obstacle.ValueRW.Obstacle = pool.Get(entity);
        
        pool.CleanUpBuild();
    }
}