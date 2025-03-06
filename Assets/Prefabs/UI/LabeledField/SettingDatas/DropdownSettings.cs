using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct DropdownSettings {
    public Color?        backgroundColor;
    public TextSettings? captionTextSettings;
    public TextSettings? itemTextSettings;
    public List<string>  itemList;
}

public static class TMP_DropdownExtensions {
    public static void SetDropdown(this TMP_Dropdown dropdown, DropdownSettings settings, Image backgroundImg) {
        if (settings.backgroundColor     != null) backgroundImg.color = settings.backgroundColor.Value;
        if (settings.captionTextSettings != null) dropdown.captionText.SetText(settings.captionTextSettings.Value);
        if (settings.itemTextSettings    != null) dropdown.captionText.SetText(settings.itemTextSettings.Value);
        if (settings.itemList != null) {
            dropdown.ClearOptions();
            dropdown.AddOptions(settings.itemList);
        }
    }
}
