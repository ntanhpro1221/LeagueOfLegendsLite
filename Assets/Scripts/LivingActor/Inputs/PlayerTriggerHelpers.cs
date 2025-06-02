using System;
using UnityEngine.InputSystem;
using static PlayerTrigger;

public static class PlayerTriggerHelpers {
    public static Key ToKeyboard(this Item slot) => slot switch {
        Item.Spell_D => Key.D
      , Item.Spell_F => Key.F
      , Item.Spell_B => Key.B

      , Item.Skill_Passive => Key.P // It will not be executed, just exist here.
      , Item.Skill_Q       => Key.Q
      , Item.Skill_W       => Key.W
      , Item.Skill_E       => Key.E
      , Item.Skill_R       => Key.R

      , Item.Item_1 => Key.Numpad1
      , Item.Item_2 => Key.Numpad2
      , Item.Item_3 => Key.Numpad3
      , Item.Item_4 => Key.Numpad4
      , Item.Item_5 => Key.Numpad5
      , Item.Item_6 => Key.Numpad6
      , Item.Item_7 => Key.Numpad7

      , _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
    };

    private static class InternalCore {
        public static int GetKeyIndex(Item key) => (int)key;

        public static int GetKeyIndex(Other key) => ITEM_COUNT + (int)key;

        public static ref T ValueRW<T>(ref Holder<T> source, int keyIndex) {
            switch (keyIndex) {
                case (int)Item.Spell_D: return ref source.Spell_D;
                case (int)Item.Spell_F: return ref source.Spell_F;
                case (int)Item.Spell_B: return ref source.Spell_B;

                case (int)Item.Skill_Passive: return ref source.Skill_Passive;
                case (int)Item.Skill_Q:       return ref source.Skill_Q;
                case (int)Item.Skill_W:       return ref source.Skill_W;
                case (int)Item.Skill_E:       return ref source.Skill_E;
                case (int)Item.Skill_R:       return ref source.Skill_R;

                case (int)Item.Item_1: return ref source.Item_1;
                case (int)Item.Item_2: return ref source.Item_2;
                case (int)Item.Item_3: return ref source.Item_3;
                case (int)Item.Item_4: return ref source.Item_4;
                case (int)Item.Item_5: return ref source.Item_5;
                case (int)Item.Item_6: return ref source.Item_6;
                case (int)Item.Item_7: return ref source.Item_7;

                case ITEM_COUNT + (int)Other.DoneReset:  return ref source.DoneReset;
                case ITEM_COUNT + (int)Other.Move:       return ref source.Move;
                case ITEM_COUNT + (int)Other.CancelMove: return ref source.CancelMove;

                default: throw new Exception($"NGDtuanh: index {keyIndex} is not exist");
            }
        }

        public static ref readonly T ValueRO<T>(in Holder<T> source, int keyIndex) {
            switch (keyIndex) {
                case (int)Item.Spell_D: return ref source.Spell_D;
                case (int)Item.Spell_F: return ref source.Spell_F;
                case (int)Item.Spell_B: return ref source.Spell_B;

                case (int)Item.Skill_Passive: return ref source.Skill_Passive;
                case (int)Item.Skill_Q:       return ref source.Skill_Q;
                case (int)Item.Skill_W:       return ref source.Skill_W;
                case (int)Item.Skill_E:       return ref source.Skill_E;
                case (int)Item.Skill_R:       return ref source.Skill_R;

                case (int)Item.Item_1: return ref source.Item_1;
                case (int)Item.Item_2: return ref source.Item_2;
                case (int)Item.Item_3: return ref source.Item_3;
                case (int)Item.Item_4: return ref source.Item_4;
                case (int)Item.Item_5: return ref source.Item_5;
                case (int)Item.Item_6: return ref source.Item_6;
                case (int)Item.Item_7: return ref source.Item_7;

                case ITEM_COUNT + (int)Other.DoneReset:  return ref source.DoneReset;
                case ITEM_COUNT + (int)Other.Move:       return ref source.Move;
                case ITEM_COUNT + (int)Other.CancelMove: return ref source.CancelMove;

                default: throw new Exception($"NGDtuanh: index {keyIndex} is not exist");
            }
        }

        /// <summary>
        /// Turn on correspond trigger.<br/>
        /// </summary>
        public static void Set(ref Full source, int keyIndex) {
            ValueRW(ref source.Event, keyIndex).Set();
            ValueRW(ref source.Code, keyIndex)++;
        }

        /// <summary>
        /// - Check is correspond trigger is on or off.<br/>
        /// - This ensures that event will be trigger with correct tick in both client and server.<br/>
        /// <br/><b>NOTE:</b> this just care about trigger, not the data comes with it. So if you need both of them,
        /// consider <see cref="GetEvent_WithData"/>.<br/>
        /// </summary>
        public static bool GetEvent_Only(in PlayerInputData input, int keyIndex) =>
            ValueRO(input.triggers.Event, keyIndex).IsSet;

        /// <summary>
        /// - Check is correspond trigger is on or off.<br/>
        /// - This ensures that an event will be executed in both client and server with <b>correct input data</b>.<br/>
        /// <br/><b>NOTE:</b> execute tick may not be the same in both client and server. So if you don't care
        /// about input data for this trigger, just use <see cref="GetEvent_Only"/>.<br/>
        /// </summary>
        public static bool GetEvent_WithData(in PlayerInputData input, in PrevCode prevCode, int keyIndex) =>
            ValueRO(input.triggers.Code, keyIndex) != ValueRO(prevCode.Code, keyIndex);
    }

#region GET TRIGGER

    public static ref T ValueRW<T>(this ref Holder<T> source, Item key) =>
        ref InternalCore.ValueRW(ref source, InternalCore.GetKeyIndex(key));

    public static ref T ValueRW<T>(this ref Holder<T> source, Other key) =>
        ref InternalCore.ValueRW(ref source, InternalCore.GetKeyIndex(key));

    public static ref readonly T ValueRO<T>(this in Holder<T> source, Item key) =>
        ref InternalCore.ValueRO(source, InternalCore.GetKeyIndex(key));

    public static ref readonly T ValueRO<T>(this in Holder<T> source, Other key) =>
        ref InternalCore.ValueRO(source, InternalCore.GetKeyIndex(key));

#endregion

#region SET TRIGGER

    /// <summary>
    /// <inheritdoc cref="InternalCore.Set"/>
    /// </summary>
    public static void Set(this ref Full full, Item key) =>
        InternalCore.Set(ref full, InternalCore.GetKeyIndex(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.Set"/>
    /// </summary>
    public static void Set(this ref Full full, Other key) =>
        InternalCore.Set(ref full, InternalCore.GetKeyIndex(key));

#endregion

#region GET TRIGGER STATE

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_Only"/>
    /// </summary>
    public static bool GetEvent_Only(this in PlayerInputData input, Item key) =>
        InternalCore.GetEvent_Only(input, InternalCore.GetKeyIndex(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_Only"/>
    /// </summary>
    public static bool GetEvent_Only(this in PlayerInputData input, Other key) =>
        InternalCore.GetEvent_Only(input, InternalCore.GetKeyIndex(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_WithData"/>
    /// </summary>
    public static bool GetEvent_WithData(this in PlayerInputData input, in PrevCode prevCode, Item key) =>
        InternalCore.GetEvent_WithData(input, prevCode, InternalCore.GetKeyIndex(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_WithData"/>
    /// </summary>
    public static bool GetEvent_WithData(this in PlayerInputData input, in PrevCode prevCode, Other key) =>
        InternalCore.GetEvent_WithData(input, prevCode, InternalCore.GetKeyIndex(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_WithData"/>
    /// </summary>
    public static bool GetEvent_WithData(this in PlayerInputAspectRO input, Item key) =>
        InternalCore.GetEvent_WithData(input.Input, input.PrevCode, InternalCore.GetKeyIndex(key));

    /// <summary>
    /// <inheritdoc cref="InternalCore.GetEvent_WithData"/>
    /// </summary>
    public static bool GetEvent_WithData(this in PlayerInputAspectRO input, Other key) =>
        InternalCore.GetEvent_WithData(input.Input, input.PrevCode, InternalCore.GetKeyIndex(key));

#endregion
}