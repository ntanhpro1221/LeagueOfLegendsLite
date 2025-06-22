using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class ChampionDataManaged : IHaveStatsPerLevelManaged, IHaveBountyManaged, IHaveSkillsManaged {
    public string                       name;
    public string                       description;
    public CovEnumMap<StatId, float_Q3> stats;
    public CovEnumMap<StatId, float_Q3> statsPerLevel;
    public Sprite                       avatar;

    public IActivableItemSO       passive;
    public List<IActivableItemSO> skills;

    public void AddAllSkillPrefabBuffer(IBaker baker, in Entity entity) {
        passive.AddPrefabBuffer(baker, entity);
        foreach (var skill in skills) skill.AddPrefabBuffer(baker, entity);
    }

    public CovEnumMap<StatId, float_Q3>   Stats         => stats;
    public CovEnumMap<StatId, float_Q3>   StatsPerLevel => statsPerLevel;
    public CovEnumMap<BountyId, float_Q3> Bounty        => GameSO.ChampCommonInitBounty;
    public IActivableItemSO               Passive       => passive;
    public List<IActivableItemSO>         Skills        => skills;
}