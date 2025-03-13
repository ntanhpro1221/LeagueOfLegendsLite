using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDataManaged {
    [HideInInspector]
    public ItemId id;

    public string             name;
    public string             description;
    public Sprite             avatar;
    public List<StatBuffData> buffs;
}