using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using UnityEngine;

[Serializable]
public class ChampionDataManaged {
    [HideInInspector]
    public ChampionId id;

    public string                               name;
    public string                               description;
    public CovEnumMap<ChampionStatsType, float> stats;
    public CovEnumMap<ChampionStatsType, float> statsPerLevel;
    public GameObject                           prefab;
    public Sprite                               avatar;
    public Sprite                               passiveAvatar;
    public List<Sprite>                         skillAvatars;
}