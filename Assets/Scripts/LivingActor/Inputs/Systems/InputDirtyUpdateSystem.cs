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
        UpdateKeyboardButtons(ref state, ref inputData);
    }

    private void UpdateRay(ref SystemState state, ref InputDirtyData inputData) {
        var ray = Camera.main!.ScreenPointToRay(Mouse.current.position.value);
        SystemAPI.GetSingletonRW<InputDirtyData>().ValueRW = new() {
            rayStart = ray.origin
          , rayEnd   = ray.GetPoint(1e5f)
        };
    }

    private void UpdateMouseButtons(ref SystemState state, ref InputDirtyData inputData) {
        var mouse = Mouse.current;

        inputData.leftMouse  = mouse.leftButton.GetButtonState();
        inputData.rightMouse = mouse.rightButton.GetButtonState();
    }

    private void UpdateKeyboardButtons(ref SystemState state, ref InputDirtyData inputData) {
        var keyboard = Keyboard.current;

        inputData.a_key = keyboard.aKey.GetButtonState();
        inputData.s_key = keyboard.sKey.GetButtonState();
        inputData.d_key = keyboard.dKey.GetButtonState();
        inputData.f_key = keyboard.fKey.GetButtonState();
        inputData.q_key = keyboard.qKey.GetButtonState();
        inputData.w_key = keyboard.wKey.GetButtonState();
        inputData.e_key = keyboard.eKey.GetButtonState();
        inputData.r_key = keyboard.rKey.GetButtonState();
    }
}