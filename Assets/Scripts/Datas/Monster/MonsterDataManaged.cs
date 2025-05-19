using System;
using NGDtuanh.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class MonsterDataManaged {
    public CovEnumMap<StatsType, float_Q3>  stats;
    public CovEnumMap<BountyType, float_Q3> bounty;
    public int                              leashRange;
    public float                            respawnCDTime;

    [FormerlySerializedAs("constTickRate")] [HideInInspector]
    public int staticTickRate;
}