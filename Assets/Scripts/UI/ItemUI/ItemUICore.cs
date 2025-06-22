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

    private void UpdateInteractable() {
        if (_ForceOffInteractable
         || _IsInCooldown
         || _IsInDead
         || _IsBlocked)
            DisablableUI.DisableAll();
        else DisablableUI.EnableAll();
    }

    private void UpdateCooldownVisible() {
        _CooldownImage.gameObject.SetActive(_IsInCooldown);
        _CooldownText.gameObject.SetActive(_IsInCooldown);
    }

    private void UpdateBlockVisible() {
        _BlockImage.enabled = _IsBlocked;
    }

    private bool _ForceOffInteractable;

    public bool ForceOffInteractable {
        get => _ForceOffInteractable;
        set {
            _ForceOffInteractable = value;

            UpdateInteractable();
        }
    }

    private bool _IsInCooldown;

    public bool IsInCooldown {
        get => _IsInCooldown;
        private set {
            _IsInCooldown = value;

            UpdateCooldownVisible();
            UpdateInteractable();
        }
    }

    private bool _IsInDead;

    public bool IsInDead {
        get => _IsInDead;
        private set {
            _IsInDead = value;

            UpdateInteractable();
        }
    }

    private bool _IsBlocked;

    public bool IsBlocked {
        get => _IsBlocked;
        private set {
            _IsBlocked = value;

            UpdateBlockVisible();
            UpdateInteractable();
        }
    }

    #region COOLDOWN

    [SerializeField] private Image           _CooldownImage;
    [SerializeField] private TextMeshProUGUI _CooldownText;

    private float _TotalCooldown;

    public void StartCooldown(float totalCooldown) {
        IsInCooldown = true;

        UpdateCooldownTime(_TotalCooldown = totalCooldown);
    }

    public void UpdateCooldownTime(float curCooldown) {
        _CooldownText.text        = ((int)curCooldown).ToString();
        _CooldownImage.fillAmount = curCooldown / _TotalCooldown;
    }

    public void DoneCooldown() {
        IsInCooldown = false;
    }

    #endregion

    #region DEAD

    public void StartDead() => IsInDead = true;

    public void DoneDead() => IsInDead = false;

    #endregion

    #region BLOCK

    public void StartBlock() => IsBlocked = true;

    public void DoneBlock() => IsBlocked = false;

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