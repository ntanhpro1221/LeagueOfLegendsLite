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

public static class BountyId {
    public const int Gold_Kill   = (int)BountyType.Gold_Kill;
    public const int Gold_Team   = (int)BountyType.Gold_Team;
    public const int Gold_Assist = (int)BountyType.Gold_Assist;
    public const int Gold_Near   = (int)BountyType.Gold_Near;
    public const int Exp_Kill    = (int)BountyType.Exp_Kill;
    public const int Exp_Team    = (int)BountyType.Exp_Team;
    public const int Exp_Assist  = (int)BountyType.Exp_Assist;
    public const int Exp_Near    = (int)BountyType.Exp_Near;
    public const int CreepScore  = (int)BountyType.CreepScore;
    public const int KillScore   = (int)BountyType.KillScore;
}