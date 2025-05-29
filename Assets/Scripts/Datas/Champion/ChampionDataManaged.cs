using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class ChampionDataManaged {
    public string                          name;
    public string                          description;
    public CovEnumMap<StatsType, float_Q3> stats;
    public CovEnumMap<StatsType, float_Q3> statsPerLevel;
    public Sprite                          avatar;

    public IActivableItemDataSO       passive;
    public List<IActivableItemDataSO> skills;

    public void AddAllSkillPrefabBuffer(IBaker baker, in Entity entity) {
        passive.AddPrefabBuffer(baker, entity);
        foreach (var skill in skills) skill.AddPrefabBuffer(baker, entity);
    }
}