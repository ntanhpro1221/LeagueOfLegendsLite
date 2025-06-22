using System;
using NGDtuanh.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class MonsterDataManaged : IHaveStatsManaged, IHaveBountyManaged {
    public CovEnumMap<StatId, float_Q3>     stats;
    public CovEnumMap<BountyId, float_Q3> bounty;
    public int                              leashRange;
    public float                            respawnCDTime;

    public CovEnumMap<StatId, float_Q3>     Stats  => stats;
    public CovEnumMap<BountyId, float_Q3> Bounty => bounty;
}