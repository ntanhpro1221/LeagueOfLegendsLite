using System;
using UnityEngine.InputSystem;

/// <summary>
/// Step to add more item:<br/>
/// - Add to this enum of course.<br/>
/// - Add to <see cref="PlayerActivableItemExtensions"/>.<br/>
/// - Add to <see cref="PlayerTrigger.Key"/>.<br/>
/// - Add to <see cref="PlayerTrigger.Item{T}"/>.<br/>
/// - Add to <see cref="PlayerTrigger.Get{T}"/>.<br/>
/// </summary>
public enum PlayerActivableItem {
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
}

public static class PlayerActivableItemExtensions {
    public static Key ToKey(this PlayerActivableItem source) => source switch {
        PlayerActivableItem.Spell_D => Key.D
      , PlayerActivableItem.Spell_F => Key.F
      , PlayerActivableItem.Spell_B => Key.B

      , PlayerActivableItem.Skill_Q => Key.Q
      , PlayerActivableItem.Skill_W => Key.W
      , PlayerActivableItem.Skill_E => Key.E
      , PlayerActivableItem.Skill_R => Key.R

      , PlayerActivableItem.Item_1 => Key.Numpad1
      , PlayerActivableItem.Item_2 => Key.Numpad2
      , PlayerActivableItem.Item_3 => Key.Numpad3
      , PlayerActivableItem.Item_4 => Key.Numpad4
      , PlayerActivableItem.Item_5 => Key.Numpad5
      , PlayerActivableItem.Item_6 => Key.Numpad6
      , PlayerActivableItem.Item_7 => Key.Numpad7

      , _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
    };
}