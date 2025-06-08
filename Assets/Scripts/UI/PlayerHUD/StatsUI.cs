using System;
using System.Globalization;
using NGDtuanh.BubleAsset;
using TMPro;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class StatsUI {
    [SerializeField] private TextMeshProUGUI
        _Physic
      , _Armor
      , _Magic
      , _MagicRes
      , _MoveSpeed
      , _Crit
      , _AtkSpeed
      , _CDReduce;

    private void UpdateSingle(
        in DynamicBuffer<StatsBuffer> source
      , TextMeshProUGUI               text
      , StatsType                     type) {
        text.text = ((int)source[(int)type].value).ToString();
    }

    public void Update(in DynamicBuffer<StatsBuffer> source) {
        UpdateSingle(source, _Physic,    StatsType.PhysicDamage);
        UpdateSingle(source, _Armor,     StatsType.Armor);
        UpdateSingle(source, _Magic,     StatsType.MagicDamage);
        UpdateSingle(source, _MagicRes,  StatsType.SpellBlock);
        UpdateSingle(source, _MoveSpeed, StatsType.MoveSpeed);
        UpdateSingle(source, _Crit,      StatsType.Crit);
        _Crit.text     += '%';
        _AtkSpeed.text =  Math.Round(source[StatsId.AttackSpeed].value, 2).ToString(CultureInfo.InvariantCulture);
    }

    public void UpdateCDReduce(int value) {
        _CDReduce.text = value.ToString();
    }
}