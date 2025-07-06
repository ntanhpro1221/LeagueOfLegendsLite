using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabeledDropdown : LabeledField {
    [field: SerializeField] public TMP_Dropdown Dropdown           { get; private set; }
    [field: SerializeField] public Image        DropdownBackground { get; private set; }

    public new LabeledDropdown WithLabel(TextSettings settings) => base.WithLabel(settings) as LabeledDropdown;

    public LabeledDropdown WithDropdown(DropdownSettings settings) {
        Dropdown.SetDropdown(settings, DropdownBackground);
        return this;
    }
}