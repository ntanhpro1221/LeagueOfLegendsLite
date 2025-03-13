using System;
using System.Collections.Generic;
using NGDtuanh.Collections.EnumMap;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class ChampionDataManaged : IComponentData {
    [HideInInspector]
    public ChampionId id;

    public string                            name;
    public string                            description;
    public EnumMap<ChampionStatsType, float> stats;
    public EnumMap<ChampionStatsType, float> statsPerLevel;
    public GameObject                        prefab;
    public Sprite                            avatar;
    public Sprite                            passiveAvatar;
    public List<Sprite>                      skillAvatars;
}