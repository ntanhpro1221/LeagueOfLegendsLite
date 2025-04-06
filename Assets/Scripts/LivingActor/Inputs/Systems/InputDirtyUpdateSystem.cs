using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(BeforePlayerInputUpdateSystemGroup))]
public partial struct InputDirtyUpdateSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.EntityManager.CreateSingleton<InputDirtyData>();
    }

    public void OnUpdate(ref SystemState state) {
        ref var inputData = ref SystemAPI.GetSingletonRW<InputDirtyData>().ValueRW;

        UpdateRay(ref state, ref inputData);
        UpdateMouseButtons(ref state, ref inputData);
    }

    private void UpdateRay(ref SystemState state, ref InputDirtyData inputData) {
        var ray = Camera.main!.ScreenPointToRay(Mouse.current.position.value);
        SystemAPI.GetSingletonRW<InputDirtyData>().ValueRW = new() {
            rayStart = ray.origin
          , rayEnd   = ray.GetPoint(1e5f)
        };
    }

    private void UpdateMouseButtons(ref SystemState state, ref InputDirtyData inputData) {
        inputData.leftMouse  = Mouse.current.leftButton.GetButtonState();
        inputData.rightMouse = Mouse.current.rightButton.GetButtonState();
    }
}