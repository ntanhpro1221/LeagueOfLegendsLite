using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;

namespace Pathfinding {
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct FixablePosServerSystem_AfterPhysics : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.Dependency = new FixPosJob()
                .ScheduleParallel(state.Dependency);
        }
    }
}