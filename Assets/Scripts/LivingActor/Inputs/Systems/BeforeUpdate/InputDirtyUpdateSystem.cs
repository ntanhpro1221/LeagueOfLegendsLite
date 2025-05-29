using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(BeforeInputLocalUpdateSystemGroup))]
public partial struct InputDirtyUpdateSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputDirtyData>();
        state.RequireForUpdate<InputDirtyData.PlayerActivableItemBuffer>();
    }

    public void OnUpdate(ref SystemState state) {
        ref var inputData = ref SystemAPI.GetSingletonRW<InputDirtyData>().ValueRW;

        UpdateRay(ref state, ref inputData);
        UpdateMouseButtons(ref state, ref inputData);
        UpdateKeyboardButtons(ref state, ref inputData
          ,                              SystemAPI.GetSingletonBuffer<InputDirtyData.PlayerActivableItemBuffer>(isReadOnly: false));
    }

    private void UpdateRay(ref SystemState state, ref InputDirtyData inputData) {
        var ray = Camera.main!.ScreenPointToRay(Mouse.current.position.value);

        inputData.mouse_ray_start = ray.origin;
        inputData.mouse_ray_end   = ray.GetPoint(1e5f);
    }

    private void UpdateMouseButtons(ref SystemState state, ref InputDirtyData inputData) {
        var mouse = Mouse.current;

        inputData.mouse_left  = mouse.leftButton.GetButtonState();
        inputData.mouse_right = mouse.rightButton.GetButtonState();
    }

    private void UpdateKeyboardButtons(ref SystemState state, ref InputDirtyData inputData, DynamicBuffer<InputDirtyData.PlayerActivableItemBuffer> inputBuffer) {
        var keyboard = Keyboard.current;

        inputData.key_a = keyboard.aKey.GetButtonState();
        inputData.key_s = keyboard.sKey.GetButtonState();

        for (int i = 0; i < inputBuffer.Length; ++i)
            inputBuffer.ElementAt(i).key = keyboard[((PlayerActivableItem)i).ToKey()].GetButtonState();
    }
}