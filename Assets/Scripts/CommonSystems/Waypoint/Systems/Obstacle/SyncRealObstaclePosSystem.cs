using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(UpdateObstacleSystemGroup))]
[UpdateAfter(typeof(ProvideRealObstacleSystem))]
public partial struct SyncObstacleSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ObstacleConfigData>();
        state.RequireForUpdate<EnumIndexData>();
    }

    public void OnUpdate(ref SystemState state) {
        int radiusId    = SystemAPI.GetSingleton<EnumIndexData>().StatsType[StatsType.UnitRadius];
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
                  , DynamicBuffer<StatsBuffer>>()) {
            // Not use x and z directly without quantization because of precision :v (just to make sure)
            obstacle.ValueRO.Obstacle.transform.position = locTrans.ValueRO.Position;
            obstacle.ValueRO.Obstacle.circleRadius       = stats[radiusId].value + radiusBonus;
        }
    }
}