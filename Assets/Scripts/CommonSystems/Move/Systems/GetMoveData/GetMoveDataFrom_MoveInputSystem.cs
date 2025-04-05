using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(BeforeMoveSystemGroup))]
public partial struct GetMoveDataFrom_MoveInputSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
                moveData
              , playerInput)
            in SystemAPI.Query<
                    RefRW<MoveData>
                  , RefRO<PlayerInputData>>()
                .WithAll<Simulate>()
                .WithNone<NetworkDestroyedTag>())
            if (playerInput.ValueRO.moveEvent.IsSet)
                moveData.ValueRW.targetLocalPos = playerInput.ValueRO.targetLocalPos;
    }
}