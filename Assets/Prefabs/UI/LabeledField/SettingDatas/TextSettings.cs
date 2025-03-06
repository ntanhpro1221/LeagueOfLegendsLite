using TMPro;
using UnityEngine;

public struct TextSettings {
    public string      text;
    public Color?      color;
    public FontWeight? fontWeight;
    public FontStyles? FontStyle;
}

public static class TMP_TextExtensions {
    public static void SetText(this TMP_Text text, TextSettings settings) {
        if (settings.text       != null) text.text       = settings.text;
        if (settings.color      != null) text.color      = settings.color.Value;
        if (settings.fontWeight != null) text.fontWeight = settings.fontWeight.Value;
        if (settings.FontStyle  != null) text.fontStyle  = settings.FontStyle.Value;
    }
}