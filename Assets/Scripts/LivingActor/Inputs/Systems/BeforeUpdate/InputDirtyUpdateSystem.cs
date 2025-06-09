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
        state.RequireForUpdate<InputDirtyData.ActivableItemBuffer>();
    }

    public void OnUpdate(ref SystemState state) {
        ref var inputData = ref SystemAPI.GetSingletonRW<InputDirtyData>().ValueRW;

        UpdateRay(ref state, ref inputData);
        UpdateMouseButtons(ref state, ref inputData);
        UpdateSkillRequest(ref state, ref inputData);
        UpdateKeyboardButtons(ref state
          , ref inputData, SystemAPI.GetSingletonBuffer<InputDirtyData.ActivableItemBuffer>(isReadOnly: false));
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

    private void UpdateSkillRequest(ref SystemState state, ref InputDirtyData inputData) {
        inputData.haveSkillUpgradeRequest = PlayerHUD.Instance.ActivableItems
            .PopOutUpdateSkillRequest(out inputData.skillToUpgrade);
    }

    private void UpdateKeyboardButtons(ref SystemState state, ref InputDirtyData inputData, DynamicBuffer<InputDirtyData.ActivableItemBuffer> inputBuffer) {
        var keyboard = Keyboard.current;

        inputData.key_a = keyboard.aKey.GetButtonState();
        inputData.key_s = keyboard.sKey.GetButtonState();

        for (int i = 0; i < PlayerTrigger.ITEM_COUNT; ++i)
            inputBuffer.ElementAt(i).key = keyboard[((PlayerTrigger.Item)i).ToKeyboard()].GetButtonState();
    }
}