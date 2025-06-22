using Unity.Entities;
using Unity.NetCode;

public static class PlayerTrigger {
    /// <summary>
    /// Put this <b>INSIDE</b> <see cref="PlayerInputData"/>.
    /// </summary>
    public struct Full {
        public Strum.PlayerTrigger.Fields<InputEvent> Event;
        public Strum.PlayerTrigger.Fields<int>        Code;
    }

    /// <summary>
    /// Put this <b>OUTSIDE</b> <see cref="PlayerInputData"/>.
    /// </summary>
    public struct PrevCode : IComponentData {
        [GhostField] public Strum.PlayerTrigger.Fields<int> Code;
    }
}

public static class PlayerTriggerHelpers {
    private static class InternalCore {
        /// <summary>
        /// Turn on correspond trigger.<br/>
        /// </summary>
        public static void Set(ref PlayerTrigger.Full source, int keyIndex) {
            source.Event.ValueRW(keyIndex).Set();
            source.Code.ValueRW(keyIndex)++;
        }

        /// <summary>
        /// - Check is correspond trigger is on or off.<br/>
        /// - This ensures that event will be trigger with correct tick in both client and server.<br/>
        /// <br/><b>NOTE:</b> this just care about trigger, not the data comes with it. So if you need both of them,
        /// consider <see cref="GetEvent_WithData"/>.<br/>
        /// </summary>
        public static bool GetEvent_Only(in PlayerInputData input, int keyIndex) =>
            input.triggers.Event.ValueRO(keyIndex).IsSet;

        /// <summary>
        /// - Check is correspond trigger is on or off.<br/>
        /// - This ensures that an event will be executed in both client and server with <b>correct input data</b>.<br/>
        /// <br/><b>NOTE:</b> execute tick may not be the same in both client and server. So if you don't care
        /// about input data for this trigger, just use <see cref="GetEvent_Only"/>.<br/>
        /// </summary>
        public static bool GetEvent_WithData(in PlayerInputData input, in PlayerTrigger.PrevCode prevCode, int keyIndex) =>
            input.triggers.Code.ValueRO(keyIndex) != prevCode.Code.ValueRO(keyIndex);
    }

    #region SET TRIGGER

    /// <summary>
    /// <inheritdoc cref="InternalCore.Set"/>
    /// </summary>
    public static void Set(this ref PlayerTrigger.Full full, SlotItemId key) =>
        InternalCore.Set(ref full, Strum.PlayerTrigger.IndexOf(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.Set"/>
    /// </summary>
    public static void Set(this ref PlayerTrigger.Full full, InputRequestId key) =>
        InternalCore.Set(ref full, Strum.PlayerTrigger.IndexOf(key));

    #endregion

    #region GET TRIGGER STATE

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_Only"/>
    /// </summary>
    public static bool GetEvent_Only(this in PlayerInputData input, SlotItemId key) =>
        InternalCore.GetEvent_Only(input, Strum.PlayerTrigger.IndexOf(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_Only"/>
    /// </summary>
    public static bool GetEvent_Only(this in PlayerInputData input, InputRequestId key) =>
        InternalCore.GetEvent_Only(input, Strum.PlayerTrigger.IndexOf(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_WithData"/>
    /// </summary>
    public static bool GetEvent_WithData(this in PlayerInputData input, in PlayerTrigger.PrevCode prevCode, SlotItemId key) =>
        InternalCore.GetEvent_WithData(input, prevCode, Strum.PlayerTrigger.IndexOf(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_WithData"/>
    /// </summary>
    public static bool GetEvent_WithData(this in PlayerInputData input, in PlayerTrigger.PrevCode prevCode, InputRequestId key) =>
        InternalCore.GetEvent_WithData(input, prevCode, Strum.PlayerTrigger.IndexOf(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_WithData"/>
    /// </summary>
    public static bool GetEvent_WithData(this in PlayerInputAspectRO input, SlotItemId key) =>
        InternalCore.GetEvent_WithData(input.Input, input.PrevCode, Strum.PlayerTrigger.IndexOf(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_WithData"/>
    /// </summary>
    public static bool GetEvent_WithData(this in PlayerInputAspectRO input, InputRequestId key) =>
        InternalCore.GetEvent_WithData(input.Input, input.PrevCode, Strum.PlayerTrigger.IndexOf(key));

    #endregion
}
