using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(UpdateObstacleSystemGroup))]
public partial struct RequestObstacleSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
        state.CompleteDependency();
    }

    [WithPresent(typeof(ActiveObstacle))]
    [BurstCompile]
    private partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(EnabledRefRW<ActiveObstacle> obstacleRequest, in MoveData moveData) {
            // Request obstacle when isn't moving
            obstacleRequest.ValueRW = moveData.isMoveDone;
        }
    }
}