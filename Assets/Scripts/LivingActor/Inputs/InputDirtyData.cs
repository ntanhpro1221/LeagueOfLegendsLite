using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem.Controls;

public struct InputDirtyData : IComponentData {
    public float3 rayStart;
    public float3 rayEnd;

    public ButtonState leftMouse;
    public ButtonState rightMouse;

    public ButtonState a_key;
    public ButtonState s_key;
    public ButtonState d_key;
    public ButtonState f_key;
    public ButtonState q_key;
    public ButtonState w_key;
    public ButtonState e_key;
    public ButtonState r_key;

    public enum ButtonState {
        None = 0
      , Down = 1
      , Up   = 2
      , Hold = 3
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