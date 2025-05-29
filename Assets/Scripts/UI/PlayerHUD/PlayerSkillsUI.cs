using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillsUI : MonoBehaviour {
    [SerializeField] private DataSOReader _SOReader;

    public ItemUI            Passive;
    public List<ItemSkillUI> Skills;
    public ItemUI            Spell_First;
    public ItemUI            Spell_Second;

    public void InitAll(ChampionId id) {
        var champData = _SOReader.Champ[id];

        Passive.InitAll(champData.passive);
        for (int i = 0; i < champData.skills.Count; i++)
            Skills[i].InitAll(champData.skills[i]);

        // TODO: Init spell later
    }
}