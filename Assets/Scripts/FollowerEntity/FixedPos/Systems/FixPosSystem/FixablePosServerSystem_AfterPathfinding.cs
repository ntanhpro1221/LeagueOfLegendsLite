using Pathfinding.ECS;
using Unity.Burst;
using Unity.Entities;

namespace Pathfinding {
    [UpdateInGroup(typeof(AIMovementSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct FixablePosServerSystem_AfterPathfinding : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.Dependency = new FixPosJob()
                .ScheduleParallel(state.Dependency);
        }
    }
}