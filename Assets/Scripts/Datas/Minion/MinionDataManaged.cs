using System;
using NGDtuanh.Collections;

[Serializable]
public class MinionDataManaged {
    public CovEnumMap<StatsType, float_Q3>  stats;
    public CovEnumMap<BountyType, float_Q3> bounty;
    public int                              aggroRange;
}