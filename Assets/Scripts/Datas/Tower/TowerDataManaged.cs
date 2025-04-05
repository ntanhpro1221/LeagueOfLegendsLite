using System;
using NGDtuanh.Collections;

[Serializable]
public class TowerDataManaged {
    public CovEnumMap<StatsType, float_Q3> stats;
    public CovEnumMap<BountyType, int>     bounty;
}