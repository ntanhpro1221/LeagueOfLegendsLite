public enum BountyType {
    Gold_Kill
  , Gold_Team
  , Gold_Assist
  , Gold_Near
  , Exp_Kill
  , Exp_Team
  , Exp_Assist
  , Exp_Near
  , CreepScore
  , KillScore

    // Just for counting
  , _COUNT
}

public static partial class EnumCount {
    public const int Bounty = (int)global::BountyType._COUNT;
}