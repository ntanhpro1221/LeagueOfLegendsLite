using UnityEngine;
using UnityEngine.UI;

public class EffectIconItemUI : MonoBehaviour {
    [SerializeField] private Image _Avatar;

    public void InitUI(EffectData.Managed.IconData data) {
        _Avatar.sprite = data.icon;
    }
}