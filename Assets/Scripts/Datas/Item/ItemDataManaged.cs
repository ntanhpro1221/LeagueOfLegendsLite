using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDataManaged {
    [HideInInspector]
    public ItemId             id;
    public List<StatBuffData> buffs;
    public string             name;
    public string             description;
    public Sprite             avatar;
}