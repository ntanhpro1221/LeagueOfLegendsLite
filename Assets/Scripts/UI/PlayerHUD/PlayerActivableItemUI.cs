using System;
using NGDtuanh.Collections;
using UnityEngine;

public class PlayerActivableItemUI : MonoBehaviour {
    public EnumMap<PlayerTrigger.Item, ItemUI> Items;

    /// <summary>
    /// Just from Q to R (not have passive).
    /// </summary>
    private EnumMap<PlayerTrigger.Item, ItemSkillUI> _Skills;

    /// <summary>
    /// <inheritdoc cref="_Skills"/>
    /// </summary>
    public EnumMap<PlayerTrigger.Item, ItemSkillUI> Skills {
        get {
            if (_Skills == null) {
                _Skills = new EnumMap<PlayerTrigger.Item, ItemSkillUI>();

                for (var skillKey = PlayerTrigger.Item.Skill_Q; skillKey <= PlayerTrigger.Item.Skill_R; ++skillKey)
                    _Skills[skillKey] = Items[skillKey].GetComponentInParent<ItemSkillUI>(includeInactive: true);
            }

            return _Skills;
        }
    }

    private PlayerTrigger.Item? _CurUpdateSkillRequest;

    private void Awake() {
        for (var skillKey = PlayerTrigger.Item.Skill_Q; skillKey <= PlayerTrigger.Item.Skill_R; ++skillKey) {
            var l_SkillKey = skillKey;
            Skills[skillKey].RegisterUpLevelListener(() => _CurUpdateSkillRequest = l_SkillKey);
        }
    }

    public bool PopOutUpdateSkillRequest(out PlayerTrigger.Item request) {
        bool haveRequest = _CurUpdateSkillRequest != null;
        request = haveRequest
            ? _CurUpdateSkillRequest.Value
            : default;
        _CurUpdateSkillRequest = null;
        return haveRequest;
    }

    public void InitAllSkills(ChampionId id) {
        var champData = GameSO.Champ[id];

        Items[PlayerTrigger.Item.Skill_Passive].InitAll(champData.passive);

        for (var skillKey = PlayerTrigger.Item.Skill_Q; skillKey <= PlayerTrigger.Item.Skill_R; ++skillKey)
            Skills[skillKey].InitAll(champData.skills[skillKey - PlayerTrigger.Item.Skill_Q]);

        // TODO: Init spell later
    }

    public void StartDeadAllItems() {
        for (PlayerTrigger.Item key = default; key < PlayerTrigger.Item.COUNT; ++key)
            Items[key].StartDead();
    }

    public void DoneDeadAllItems() {
        for (PlayerTrigger.Item key = default; key < PlayerTrigger.Item.COUNT; ++key)
            Items[key].DoneDead();
    }
}