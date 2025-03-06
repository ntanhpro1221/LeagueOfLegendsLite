using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct InputSettings {
    public TMP_InputField.ContentType? contentType;
    public Color?                      backgroundColor;
    public TextSettings?               placeholderSettings;
    public TextSettings?               inputTextSettings;
}

public static class TMP_InputFieldExtensions {
    public static void SetInput(this TMP_InputField input, InputSettings settings, Image backgroundImg) {
        if (settings.contentType         != null) input.contentType   = settings.contentType.Value;
        if (settings.backgroundColor     != null) backgroundImg.color = settings.backgroundColor.Value;
        if (settings.placeholderSettings != null) (input.placeholder as TMP_Text)?.SetText(settings.placeholderSettings.Value);
        if (settings.inputTextSettings   != null) input.textComponent.SetText(settings.inputTextSettings.Value);
    }
}
