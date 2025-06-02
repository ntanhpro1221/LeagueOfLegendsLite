using NGDtuanh.Collections;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class PlayerActivableItemUI : MonoBehaviour {
    public EnumMap<PlayerTrigger.Item, ItemUI> Items;

    [HideInInspector] public ItemSkillUI Skill_Q;
    [HideInInspector] public ItemSkillUI Skill_W;
    [HideInInspector] public ItemSkillUI Skill_E;
    [HideInInspector] public ItemSkillUI Skill_R;

    private void Awake() {
        Skill_Q = Items[PlayerTrigger.Item.Skill_Q].GetComponentInParent<ItemSkillUI>();
        Skill_W = Items[PlayerTrigger.Item.Skill_W].GetComponentInParent<ItemSkillUI>();
        Skill_E = Items[PlayerTrigger.Item.Skill_E].GetComponentInParent<ItemSkillUI>();
        Skill_R = Items[PlayerTrigger.Item.Skill_R].GetComponentInParent<ItemSkillUI>();
    }

    public void InitAllSkills(ChampionId id) {
        var champData = GameSO.Champ[id];

        Items[PlayerTrigger.Item.Skill_Passive].InitAll(champData.passive);
        Items[PlayerTrigger.Item.Skill_Q].InitAll(champData.skills[0]);
        Items[PlayerTrigger.Item.Skill_W].InitAll(champData.skills[1]);
        Items[PlayerTrigger.Item.Skill_E].InitAll(champData.skills[2]);
        Items[PlayerTrigger.Item.Skill_R].InitAll(champData.skills[3]);

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