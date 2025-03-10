using System;
using System.Collections.Generic;
using NGDtuanh.PropertySet;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class ChampionDataManaged : IComponentData {
    [HideInInspector]
    public ChampionId                            id;
    public PropertySet<ChampionStatsType, float> stats;
    public PropertySet<ChampionStatsType, float> statsPerLevel;
    public GameObject                            prefab;
    public string                                name;
    public string                                description;
    public Sprite                                avatar;
    public Sprite                                passiveAvatar;
    public List<Sprite>                          skillAvatars;
}