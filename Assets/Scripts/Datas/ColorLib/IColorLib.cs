using System;
using NGDtuanh.Collections;
using UnityEngine;

public abstract class IColorLib<TEnum> : ScriptableObject where TEnum : struct, Enum {
    protected const string ASSET_PATH = "Color Lib/";

    [field: SerializeField] public EnumMap<TEnum, Color> Lib { get; set; }
}