using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(BeforeMoveSystemGroup))]
public partial struct GetMoveDataFrom_MoveInputSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
                moveData
              , moveInput
              , localTrans)
            in SystemAPI.Query<
                    RefRW<MoveData>
                  , RefRO<MoveInputData>
                  , RefRO<LocalTransform>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag, MoveControlDisabled>())
            moveData.ValueRW.targetLocalPos = moveInput.ValueRO.initialized
                ? moveInput.ValueRO.targetLocalPos
                : localTrans.ValueRO.Position.Quantizate3();
    }
}