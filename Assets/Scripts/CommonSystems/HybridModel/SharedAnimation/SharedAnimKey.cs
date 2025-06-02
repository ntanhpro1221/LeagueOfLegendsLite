/// <summary>
/// Step to add more key:<br/>
/// - Add to this enum.<br/>
/// - Add to <see cref="SharedAnimKeyExtensions.KeyName"/>.<br/>
/// - Add correspond [bool | speed | transition] to animator base.
/// </summary>
public enum SharedAnimKey {
    Attack = 0
  , Dead   = 1
  , Idle   = 2
  , Move   = 3

  , Idle2Dead = 4
  , Dead2Idle = 5

  , Skill_Q = 6
  , Skill_W = 7
  , Skill_E = 8
  , Skill_R = 9
}