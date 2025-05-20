using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(UpdateObstacleSystemGroup))]
public partial struct PrepareObstacleDataSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ObstacleConfigData>(); // void run in wrong scene
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        // Update active status, position of obstacle and add its position to obstaclePosArr if it is activated
        state.Dependency = new UpdateActiveObstacleDataJob()
            .ScheduleParallel(state.Dependency);
    }

    [WithPresent(
        typeof(ActiveObstacle)
      , typeof(Simulate))]
    [BurstCompile]
    public partial struct UpdateActiveObstacleDataJob : IJobEntity {
        [BurstCompile]
        public void Execute(
            EnabledRefRW<ActiveObstacle> obstacleRequest
          , MoveRequesterAspect          moveRequester
          , EnabledRefRO<Simulate>       simulateTrigger
          , in LocalTransform            locTrans) {
            // only update if it is simulating
            if (simulateTrigger.ValueRO) {
                // Request obstacle when isn't moving and its position is valid
                obstacleRequest.ValueRW =
                    moveRequester.IsMoveDone
                 && !locTrans.Position.IsAnyNaN();
            }
        }
    }
}