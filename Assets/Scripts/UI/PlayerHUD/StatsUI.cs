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
        in  DynamicBuffer<StatsBuffer> source
      , ref BubleEnMap<StatsType, int> index
      , TextMeshProUGUI                text
      , StatsType                      type) {
        text.text = ((int)source[index[type]].value).ToString();
    }

    public void Update(in DynamicBuffer<StatsBuffer> source, ref BubleEnMap<StatsType, int> index) {
        UpdateSingle(source, ref index, _Physic,    StatsType.PhysicDamage);
        UpdateSingle(source, ref index, _Armor,     StatsType.Armor);
        UpdateSingle(source, ref index, _Magic,     StatsType.MagicDamage);
        UpdateSingle(source, ref index, _MagicRes,  StatsType.SpellBlock);
        UpdateSingle(source, ref index, _MoveSpeed, StatsType.MoveSpeed);
        UpdateSingle(source, ref index, _Crit,      StatsType.Crit);
        _Crit.text     += '%';
        _AtkSpeed.text =  Math.Round(source[index[StatsType.AttackSpeed]].value, 2).ToString(CultureInfo.InvariantCulture);
    }

    public void UpdateCDReduce(int value) {
        _CDReduce.text = value.ToString();
    }
}