using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabeledInput : LabeledField {
    [field: SerializeField] public TMP_InputField Input           { get; private set; }
    [field: SerializeField] public Image          InputBackground { get; private set; }

    public new LabeledInput WithLabel(TextSettings settings) => base.WithLabel(settings) as LabeledInput;

    public LabeledInput WithInput(InputSettings settings) {
        Input.SetInput(settings, InputBackground);
        return this;
    }
}