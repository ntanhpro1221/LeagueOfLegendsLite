using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(BeforeInputLocalUpdateSystemGroup))]
public partial struct InputDirtyUpdateSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<InputDirtyData>();
    }

    public void OnUpdate(ref SystemState state) {
        ref var inputData = ref SystemAPI.GetSingletonRW<InputDirtyData>().ValueRW;

        UpdateRay(ref state, ref inputData);
        UpdateMouseButtons(ref state, ref inputData);
        UpdatePlayerRequest(ref state, ref inputData);
        UpdateKeyboardButtons(ref state, ref inputData);
        inputData.isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
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

    private void UpdatePlayerRequest(ref SystemState state, ref InputDirtyData inputData) {
        var requestHub = PlayerRequestHub.Instance;

        foreach (var request in Strum.InputRequest.Indexes)
            inputData.requestTrigger[request] = requestHub.PopEvent(request);

        inputData.requestData = requestHub.Data;
    }

    private void UpdateKeyboardButtons(ref SystemState state, ref InputDirtyData inputData) {
        var keyboard = Keyboard.current;

        inputData.key_a = keyboard.aKey.GetButtonState();
        inputData.key_s = keyboard.sKey.GetButtonState();

        foreach (var key in Strum.SlotItem.Indexes)
            inputData.activableItem[key] = keyboard[key.ToKeyboard()].GetButtonState();
    }
}