using System;
using NGDtuanh.Collections;

[Serializable]
public class TowerDataManaged : IHaveStatsManaged, IHaveBountyManaged {
    public CovEnumMap<StatId, float_Q3>     stats;
    public CovEnumMap<BountyId, float_Q3> bounty;

    public CovEnumMap<StatId, float_Q3>     Stats  => stats;
    public CovEnumMap<BountyId, float_Q3> Bounty => bounty;
}