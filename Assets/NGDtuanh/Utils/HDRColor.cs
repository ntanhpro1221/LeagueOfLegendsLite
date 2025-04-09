using System;
using UnityEngine;

[Serializable]
public struct HDRColor {
    [ColorUsage(true, true)]
    public Color Value;

    public HDRColor(Color color) => Value = color;

    public static implicit operator Color(HDRColor color) => color.Value;
    public static implicit operator HDRColor(Color color) => new(color);
}