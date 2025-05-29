using System;
using Unity.Burst;
using Unity.NetCode;

[BurstCompile]
public static class PlayerTrigger {
    /// <summary>
    /// Manual inherit from <see cref="PlayerActivableItem"/>.<br/>
    /// To add more trigger (not from <see cref="PlayerActivableItem"/>), follow these steps:<br/>
    /// - Add to this enum.<br/>
    /// - Add to <see cref="Item{T}"/>.<br/>
    /// - Add to <see cref="Get{T}"/>.<br/>
    /// </summary>
    public enum Key {
        Spell_D
      , Spell_F
      , Spell_B

      , Skill_Q
      , Skill_W
      , Skill_E
      , Skill_R

      , Item_1
      , Item_2
      , Item_3
      , Item_4
      , Item_5
      , Item_6
      , Item_7

      , DoneReset
      , Move
      , CancelMove
    }

    /// <summary>
    /// See <see cref="Key"/> to add more item.
    /// </summary>
    public struct Item<T> where T : struct {
        public T Spell_D;
        public T Spell_F;
        public T Spell_B;

        public T Skill_Q;
        public T Skill_W;
        public T Skill_E;
        public T Skill_R;

        public T Item_1;
        public T Item_2;
        public T Item_3;
        public T Item_4;
        public T Item_5;
        public T Item_6;
        public T Item_7;

        public T DoneReset;
        public T Move;
        public T CancelMove;
    }

    [BurstCompile]
    public static ref T Get<T>(this ref Item<T> source, Key key) where T : struct {
        switch (key) {
            case Key.Spell_D: return ref source.Spell_D;
            case Key.Spell_F: return ref source.Spell_F;
            case Key.Spell_B: return ref source.Spell_B;

            case Key.Skill_Q: return ref source.Skill_Q;
            case Key.Skill_W: return ref source.Skill_W;
            case Key.Skill_E: return ref source.Skill_E;
            case Key.Skill_R: return ref source.Skill_R;

            case Key.Item_1: return ref source.Item_1;
            case Key.Item_2: return ref source.Item_2;
            case Key.Item_3: return ref source.Item_3;
            case Key.Item_4: return ref source.Item_4;
            case Key.Item_5: return ref source.Item_5;
            case Key.Item_6: return ref source.Item_6;
            case Key.Item_7: return ref source.Item_7;

            case Key.DoneReset:  return ref source.DoneReset;
            case Key.Move:       return ref source.Move;
            case Key.CancelMove: return ref source.CancelMove;

            default: throw new ArgumentOutOfRangeException(nameof(key), key, null);
        }
    }

    public struct Full {
        public Item<InputEvent> Event;
        public Item<int>        Code;

        public void Set(Key key) {
            Event.Get(key).Set();
            Code.Get(key)++;
        }

        public bool GetFull(Key key, ref PlayerInputPrevCode prevCode) =>
            Event.Get(key).IsSet
         && Code.Get(key) != prevCode.Code.Get(key);
    }
}