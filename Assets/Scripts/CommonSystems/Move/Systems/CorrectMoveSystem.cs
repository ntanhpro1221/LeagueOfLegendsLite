using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial struct CorrectMoveSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
                moveData
              , localTrans
              , velocity)
            in SystemAPI.Query<
                RefRO<MoveData>
              , RefRW<LocalTransform>
              , RefRW<PhysicsVelocity>>()) {
            if (!moveData.ValueRO.isDone) return;
            localTrans.ValueRW.Position.AssignKeepY(moveData.ValueRO.targetLocalPos.Full);
            velocity.ValueRW.Linear.AssignKeepY(float3.zero);
        }
    }
}