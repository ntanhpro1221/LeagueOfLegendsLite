using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial struct CorrectMoveSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new Job()
            .ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(Simulate))]
    [BurstCompile]
    public partial struct Job : IJobEntity {
        [BurstCompile]
        public void Execute(in MoveData moveData, ref LocalTransform locTrans, ref PhysicsVelocity velocity) {
            if (moveData is {
                isFixedPos: false
              , isMoveDone: false
            }) return;
            
            locTrans.Position.AssignKeepY(moveData.fixedPos);
            GameHelpers.AssignLinearVelocity(ref velocity, float3.zero, moveData.controlYAxis);
        }
    }
}