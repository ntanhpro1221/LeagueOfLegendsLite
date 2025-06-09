using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[BurstCompile]
public static class PlayerTrigger {
    public const int ITEM_COUNT = (int)Item.COUNT;

    /// <summary>
    /// <inheritdoc cref="Other"/>
    /// - Add to <see cref="PlayerTriggerHelpers.ToKeyboard"/>.<br/>
    /// </summary>
    public enum Item {
        Spell_D
      , Spell_F
      , Spell_B

      , Skill_Passive
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

        // Count for this enum
      , COUNT
    }

    /// <summary>
    /// Steps to add move event:<br/>
    /// - Add to this enum.<br/>
    /// - Add to <see cref="Holder{T}"/>.<br/>
    /// - Add to <see cref="PlayerTriggerHelpers.InternalCore.ValueRW{T}"/>.<br/>
    /// - Add to <see cref="PlayerTriggerHelpers.InternalCore.ValueRO{T}"/>.<br/>
    /// </summary>
    public enum Other {
        DoneReset
      , Move
      , CancelMove
      , UpgradeSkill

        // Count for this enum
      , COUNT
    }

    /// <summary>
    /// All trigger from <see cref="Item"/> and <see cref="Other"/>
    /// </summary>
    public struct Holder<T> {
        public T Spell_D;
        public T Spell_F;
        public T Spell_B;

        public T Skill_Passive;
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
        public T UpgradeSkill;
    }

    /// <summary>
    /// Put this <b>INSIDE</b> <see cref="PlayerInputData"/>.
    /// </summary>
    public struct Full {
        public Holder<InputEvent> Event;
        public Holder<int>        Code;
    }

    /// <summary>
    /// Put this <b>OUTSIDE</b> <see cref="PlayerInputData"/>.
    /// </summary>
    public struct PrevCode : IComponentData {
        [GhostField] public Holder<int> Code;
    }
}