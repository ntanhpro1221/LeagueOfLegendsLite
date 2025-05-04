using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

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
              , stats
              , entity)
            in SystemAPI
                .Query<
                    RefRW<ActiveObstacle>
                  , RefRO<LocalTransform>
                  , DynamicBuffer<StatsBuffer>>()
                .WithAll<Simulate>()
                .WithEntityAccess()) {
            if (locTrans.ValueRO.Position.IsNaN())
                Debug.LogWarning($"NGDtuanh: {state.WorldName()} position of entity({entity.Index}) is NaN => {locTrans.ValueRO.Position}");
            else obstacle.ValueRW.Obstacle.transform.position = locTrans.ValueRO.Position;
            obstacle.ValueRW.Obstacle.circleRadius       = stats[radiusId].value + radiusBonus;
        }
    }
}