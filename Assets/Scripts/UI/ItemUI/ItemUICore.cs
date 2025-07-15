using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DisablableUIRoot))]
public class ItemUICore : MonoBehaviour {
    [SerializeField] private Image _BlockImage;

    private DisablableUIRoot _DisablableUI;

    private DisablableUIRoot DisablableUI {
        get {
            if (_DisablableUI == null) _DisablableUI = GetComponent<DisablableUIRoot>();
            return _DisablableUI;
        }
    }

    private Strum.ItemUIDisableFactor.Fields<bool> _DisableFactor;

    public ref readonly Strum.ItemUIDisableFactor.Fields<bool> DisableFactor => ref _DisableFactor;

    private void UpdateInteractable() {
        foreach (var disable in _DisableFactor)
            if (disable) {
                DisablableUI.DisableAll();
                return;
            }

        DisablableUI.EnableAll();
    }

    private void UpdateCooldownVisible(bool enable) {
        _CooldownImage.gameObject.SetActive(enable);
        _CooldownText.gameObject.SetActive(enable);
    }

    private void UpdateBlockVisible(bool enable) {
        _BlockImage.enabled = enable;
    }

    private bool _ForceOffInteractable;

    public void SetDisableFactor(ItemUIDisableFactor factor, bool newValue) {
        if (_DisableFactor[factor] == newValue) return;

        _DisableFactor[factor] = newValue;
        UpdateInteractable();

        switch (factor) {
            case ItemUIDisableFactor.NotEnoughLevel: break;
            case ItemUIDisableFactor.InCooldown:     UpdateCooldownVisible(newValue); break;
            case ItemUIDisableFactor.InDead:         break;
            case ItemUIDisableFactor.Blocked:        UpdateBlockVisible(newValue); break;
            case ItemUIDisableFactor.NotSatisCond:   break;

            default: throw new ArgumentOutOfRangeException(nameof(factor), factor, null);
        }
    }

    #region COOLDOWN

    [SerializeField] private Image           _CooldownImage;
    [SerializeField] private TextMeshProUGUI _CooldownText;

    private float _TotalCooldown;

    public void StartCooldown(float totalCooldown) {
        SetDisableFactor(ItemUIDisableFactor.InCooldown, true);

        UpdateCooldownTime(_TotalCooldown = totalCooldown);
    }

    public void UpdateCooldownTime(float curCooldown) {
        _CooldownText.text        = ((int)curCooldown).ToString();
        _CooldownImage.fillAmount = curCooldown / _TotalCooldown;
    }

    public void DoneCooldown() => SetDisableFactor(ItemUIDisableFactor.InCooldown, false);

    #endregion

    #region AVATAR

    [field: SerializeField] public Image Avatar { get; private set; }

    #endregion

    #region STACK

    [field: SerializeField] public TextMeshProUGUI StackText { get; private set; }

    [HideInInspector] public int CurStack;

    public bool HaveStack {
        set => StackText.gameObject.SetActive(value);
    }

    #endregion

    #region ACTIVE KEY

    [field: SerializeField] public TextMeshProUGUI ActiveKeyText { get; private set; }

    #endregion

    #region BORDER

    [field: SerializeField] public Image Border { get; private set; }

    #endregion
}