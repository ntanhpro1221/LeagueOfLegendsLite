using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public struct InputDirtyData : IComponentData {
    public struct ActivableItemBuffer : IBufferElementData {
        public ButtonState key;
    }

    public float3 mouse_ray_start;
    public float3 mouse_ray_end;

    public ButtonState mouse_left;
    public ButtonState mouse_right;

    public ButtonState key_a;
    public ButtonState key_s;
    
    public enum ButtonState {
        None = 0
      , Down = 1
      , Up   = 2
      , Hold = 3
    }
}

public class InputDirtyAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<InputDirtyAuthoring> {
        public override void Bake(InputDirtyAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<InputDirtyData>(entity);
            AddCleanBuffer<InputDirtyData.ActivableItemBuffer>(entity
              , Enum.GetValues(typeof(PlayerTrigger.Item)).Length);
        }
    }
}

public static class InputDirtyDataExtensions {
    public static InputDirtyData.ButtonState GetButtonState(this ButtonControl buttonCtrl) {
        if (buttonCtrl.wasPressedThisFrame) return InputDirtyData.ButtonState.Down;
        if (buttonCtrl.wasReleasedThisFrame) return InputDirtyData.ButtonState.Up;
        if (buttonCtrl.isPressed) return InputDirtyData.ButtonState.Hold;
        return InputDirtyData.ButtonState.None;
    } 
    
    public static bool WasPressedThisFrame(this InputDirtyData.ButtonState state)
        => state == InputDirtyData.ButtonState.Down;

    public static bool WasReleasedThisFrame(this InputDirtyData.ButtonState state)
        => state == InputDirtyData.ButtonState.Up;

    public static bool IsHolding(this InputDirtyData.ButtonState state)
        => state is
            InputDirtyData.ButtonState.Hold
         or InputDirtyData.ButtonState.Down;
}