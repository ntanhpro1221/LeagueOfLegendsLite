using NGDtuanh.Collections;
using UnityEngine;

public class PlayerActivableItemUI : MonoBehaviour {
    [field: SerializeField] public EnumMap<SlotItemId, IItemUIWrapper> Items { get; private set; }

    private void Awake_SkillRequestListener() {
        for (var skillKey = Strum.SlotItem.First_SkillNotPassive
             ; skillKey <= Strum.SlotItem.Last_Skill
             ; ++skillKey) {
            var l_SkillKey = skillKey;
            ((ItemSkillUI)Items[skillKey]).RegisterUpLevelListener(() => PlayerRequestHub.Instance.SetUpdateSkill(l_SkillKey));
        }
    }

    private void Awake_ItemSlotId() {
        for (var slotKey = Strum.SlotItem.First_Item
             ; slotKey <= Strum.SlotItem.Last_Item
             ; ++slotKey)
            ((ItemUI)Items[slotKey]).SetSlot(slotKey);
    }

    private void Awake() {
        Awake_SkillRequestListener();
        Awake_ItemSlotId();
    }

    public void InitAllSkills(ChampionId id) {
        var champData = GameSO.Champ[id];

        ((ItemSkillUI)Items[SlotItemId.Skill_Passive]).InitAll(champData.passive);

        for (var skillKey = SlotItemId.Skill_Q; skillKey <= SlotItemId.Skill_R; ++skillKey)
            ((ItemSkillUI)Items[skillKey]).InitAll(champData.skills[skillKey - SlotItemId.Skill_Q]);

        // TODO: Init spell later
    }

    #region DEAD

    private void SetDeadAllItems(bool dead) {
        foreach (var key in Strum.SlotItem.Indexes)
            Items[key].Core.SetDisableFactor(ItemUIDisableFactor.InDead, dead);
    }

    public void StartDeadAllItems() => SetDeadAllItems(true);
    public void DoneDeadAllItems()  => SetDeadAllItems(false);

    #endregion

    #region BLOCK ITEM ACTIVATION

    public bool BlockedAllItems { get; private set; }

    private void SetBlockAllItems(bool blocked) {
        BlockedAllItems = blocked;
        foreach (var key in Strum.SlotItem.Indexes)
            Items[key].Core.SetDisableFactor(ItemUIDisableFactor.Blocked, blocked);
    }

    public void StartBlockAllItems() => SetBlockAllItems(true);

    public void DoneBlockAllItems() => SetBlockAllItems(false);

    #endregion
}