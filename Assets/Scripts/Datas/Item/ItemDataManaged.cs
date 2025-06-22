using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDataManaged {
    public IActivableItemSO                  common;
    public List<StatBuffs.ElementUnscalable> buffs;
    public List<ItemId>                      recipe;
    public IndicatorConcreteBase             indicator;
    public float                             lifeTime;
    public ItemData.Settings                 settings;
}