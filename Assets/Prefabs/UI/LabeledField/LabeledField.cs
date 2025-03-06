using TMPro;
using UnityEngine;

public class LabeledField : MonoBehaviour {
    [field: SerializeField] public TextMeshProUGUI Label { get; private set; }

    public LabeledField WithLabel(TextSettings settings) {
        Label.SetText(settings);
        return this;
    }
}