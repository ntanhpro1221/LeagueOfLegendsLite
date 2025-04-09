using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(BeforeMoveSystemGroup))]
public partial struct GetMoveDataFrom_PlayerInputSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
                moveData
              , playerInput
              , localTrans)
            in SystemAPI.Query<
                    RefRW<MoveData>
                  , RefRO<PlayerInputData>
                  , RefRO<LocalTransform>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>()) {
            // apply move event
            if (playerInput.ValueRO.moveEvent.IsSet)
                moveData.ValueRW.targetLocalPos = playerInput.ValueRO.moveLocalTarget;
            
            // cancel move event
            if (playerInput.ValueRO.cancelMoveEvent.IsSet)
                moveData.ValueRW.targetLocalPos = localTrans.ValueRO.Position.Quantizate3();
        }
    }
}