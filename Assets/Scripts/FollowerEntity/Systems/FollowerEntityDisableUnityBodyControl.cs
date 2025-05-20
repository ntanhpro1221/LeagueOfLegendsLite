using Pathfinding.ECS;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct FollowerEntityDisableUnityBodyControl : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job().ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(SimulateMovement))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(ref PhysicsVelocity velocity) {
            velocity.Linear.AssignKeepY(float3.zero);
            velocity.Angular = float3.zero;
        }
    }
}