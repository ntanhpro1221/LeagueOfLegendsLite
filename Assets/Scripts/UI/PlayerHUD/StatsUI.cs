using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[Serializable]
public class StatsUI : MonoBehaviour {
    [Header("BASIC")]
    [SerializeField] private TextMeshProUGUI _PhysicDamage;

    [SerializeField] private TextMeshProUGUI _Armor;
    [SerializeField] private TextMeshProUGUI _AttackSpeed;
    [SerializeField] private TextMeshProUGUI _CritChance;
    [SerializeField] private TextMeshProUGUI _MagicDamage;
    [SerializeField] private TextMeshProUGUI _MagicRes;
    [SerializeField] private TextMeshProUGUI _AbilityHaste;
    [SerializeField] private TextMeshProUGUI _MoveSpeed;

    [Header("ADVANCE"), Space]
    [SerializeField] private GameObject _AdvanceRoot;

    [SerializeField] private TextMeshProUGUI _HealthManaRegen;
    [SerializeField] private TextMeshProUGUI _ArmorPen;
    [SerializeField] private TextMeshProUGUI _LifeSteal;
    [SerializeField] private TextMeshProUGUI _AttackRange;
    [SerializeField] private TextMeshProUGUI _HealShieldPower;
    [SerializeField] private TextMeshProUGUI _MagicPen;
    [SerializeField] private TextMeshProUGUI _Omnivamp;
    [SerializeField] private TextMeshProUGUI _Tenacity;

    private static KeyControl _CKey;

    private static KeyControl CKey => _CKey ??= Keyboard.current.cKey;

    private static void UpdateSimpleInt(
        in StatsData    source
      , TextMeshProUGUI text
      , StatId          type
      , string          suffix = "") {
        text.text = $"{(int)source.data[type]}{suffix}";
    }

    private void Update() {
        // Toggle advance
        _AdvanceRoot.SetActive(CKey.isPressed);
    }

    public void UpdateUI(in StatsData source) {
        ref readonly var data = ref source.data;

        // BASIC
        _PhysicDamage.text = $"{data.PhysicDamage:int}";
        _Armor.text        = $"{data.Armor:int}";
        _AttackSpeed.text  = $"{data.AttackSpeed:float2}";
        _CritChance.text   = $"{data.CritChance:percent}";
        _MagicDamage.text  = $"{data.MagicDamage:int}";
        _MagicRes.text     = $"{data.MagicRes:int}";
        _AbilityHaste.text = $"{data.AbilityHaste:int}";
        _MoveSpeed.text    = $"{data.MoveSpeed:int}";

        // ADVANCE
        _HealthManaRegen.text = $"{data.HealthRegen:int}|{data.ManaRegen:int}";
        _ArmorPen.text        = $"{data.ArmorPen:int}";
        _LifeSteal.text       = $"{data.LifeSteal:percent}";
        _AttackRange.text     = $"{data.AttackRange:int}";
        _HealShieldPower.text = $"{data.HealShieldPower:percent}";
        _MagicPen.text        = $"{data.MagicPen:int}";
        _Omnivamp.text        = $"{data.Omnivamp:percent}";
        _Tenacity.text        = $"{data.Tenacity:percent}";
    }
} 