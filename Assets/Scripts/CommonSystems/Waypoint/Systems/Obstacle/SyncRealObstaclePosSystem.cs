using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(UpdateObstacleSystemGroup))]
[UpdateAfter(typeof(ProvideRealObstacleSystem))]
public partial struct SyncObstacleSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ObstacleConfigData>();
    }

    public void OnUpdate(ref SystemState state) {
        if (BattleSceneLife.Instance == null) return;
        
        int radiusBonus = SystemAPI.GetSingleton<ObstacleConfigData>().radiusBonus;

        // Not only simulating entity, we must update for all of them
        foreach (var (
                obstacle
              , locTrans
              , stats)
            in SystemAPI
                .Query<
                    RefRO<ActiveObstacle>
                  , RefRO<LocalTransform>
                  , RefRO<StatsData>>()) {
            // Not use x and z directly without quantization because of precision :v (just to make sure)
            obstacle.ValueRO.Obstacle.transform.position = locTrans.ValueRO.Position;
            obstacle.ValueRO.Obstacle.circleRadius       = stats.ValueRO.data.UnitRadius + radiusBonus;
        }
    }
}