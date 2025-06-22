using System;
using UnityEngine.InputSystem;

public static partial class Strum {
    [NGDtuanh.Utils.Strum(typeof(SlotItemId))]
    public static partial class SlotItem {
        public const SlotItemId First_Spell = SlotItemId.Spell_D;
        public const SlotItemId Last_Spell  = SlotItemId.Spell_B;

        public const SlotItemId First_Skill           = SlotItemId.Skill_Passive;
        public const SlotItemId Last_Skill            = SlotItemId.Skill_R;
        public const SlotItemId First_SkillNotPassive = SlotItemId.Skill_Q;

        public const SlotItemId First_Item = SlotItemId.Item_1;
        public const SlotItemId Last_Item  = SlotItemId.Item_7;
    }
}

/// <summary>
/// When add new item to this enum:<br/>
/// - Check constant in <see cref="SlotItemIdExtensions"/>.<br/>
/// - Add to <see cref="SlotItemIdExtensions.ToKeyboard"/>.<br/>
/// </summary>
public enum SlotItemId {
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
}

public static class SlotItemIdExtensions {
    public static bool IsSpell(this SlotItemId id) => id is >= Strum.SlotItem.First_Spell and <= Strum.SlotItem.Last_Spell;
    public static bool IsSkill(this SlotItemId id) => id is >= Strum.SlotItem.First_Skill and <= Strum.SlotItem.Last_Skill;
    public static bool IsItem(this  SlotItemId id) => id is >= Strum.SlotItem.First_Item and <= Strum.SlotItem.Last_Item;

    public static Key ToKeyboard(this SlotItemId slot) => slot switch {
        SlotItemId.Spell_D => Key.D
      , SlotItemId.Spell_F => Key.F
      , SlotItemId.Spell_B => Key.B

      , SlotItemId.Skill_Passive => Key.P // It will not be executed, just exist here.
      , SlotItemId.Skill_Q       => Key.Q
      , SlotItemId.Skill_W       => Key.W
      , SlotItemId.Skill_E       => Key.E
      , SlotItemId.Skill_R       => Key.R

      , SlotItemId.Item_1 => Key.Digit1
      , SlotItemId.Item_2 => Key.Digit2
      , SlotItemId.Item_3 => Key.Digit3
      , SlotItemId.Item_4 => Key.Digit4
      , SlotItemId.Item_5 => Key.Digit5
      , SlotItemId.Item_6 => Key.Digit6
      , SlotItemId.Item_7 => Key.Digit7

      , _ => throw new ArgumentOutOfRangeException(nameof(slot), slot
          , $"NGDtuanh error value of {nameof(SlotItemId)}, founded: {slot} {(int)slot}")
    };
}