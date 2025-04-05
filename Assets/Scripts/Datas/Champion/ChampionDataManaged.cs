using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using UnityEngine;

[Serializable]
public class ChampionDataManaged {
    public string                          name;
    public string                          description;
    public CovEnumMap<StatsType, float_Q3> stats;
    public CovEnumMap<StatsType, float_Q3> statsPerLevel;
    public Sprite                          avatar;
    public Sprite                          passiveAvatar;
    public List<Sprite>                    skillAvatars;
}