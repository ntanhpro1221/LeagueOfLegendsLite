using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem.Controls;

public struct InputDirtyData : IComponentData {
    public float3 rayStart;
    public float3 rayEnd;

    public ButtonState leftMouse;
    public ButtonState rightMouse;

    public enum ButtonState {
        None
      , Down
      , Up
    }
}

public static class InputDirtyDataExtensions {
    public static InputDirtyData.ButtonState GetButtonState(this ButtonControl buttonCtrl) {
        if (buttonCtrl.wasPressedThisFrame) return InputDirtyData.ButtonState.Down;
        if (buttonCtrl.wasReleasedThisFrame) return InputDirtyData.ButtonState.Up;
        return InputDirtyData.ButtonState.None;
    }
}