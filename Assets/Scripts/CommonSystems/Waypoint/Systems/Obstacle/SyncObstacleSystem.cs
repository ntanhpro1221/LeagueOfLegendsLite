using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(UpdateObstacleSystemGroup))]
[UpdateAfter(typeof(ProvideObstacleSystem))]
public partial struct SyncObstacleSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ObstacleConfigData>();
        state.RequireForUpdate<EnumIndexData>();
    }

    public void OnUpdate(ref SystemState state) {
        int radiusId    = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.UnitRadius];
        int radiusBonus = SystemAPI.GetSingleton<ObstacleConfigData>().radiusBonus;

        foreach (var (
                obstacle
              , locTrans
              , stats)
            in SystemAPI
                .Query<
                    RefRW<ActiveObstacle>
                  , RefRO<LocalTransform>
                  , DynamicBuffer<StatsBuffer>>()
                .WithAll<Simulate>()) {
            obstacle.ValueRW.Obstacle.transform.position = locTrans.ValueRO.Position;
            obstacle.ValueRW.Obstacle.circleRadius       = stats[radiusId].value + radiusBonus;
        }
    }
}