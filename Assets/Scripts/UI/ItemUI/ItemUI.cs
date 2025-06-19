using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[RequireComponent(typeof(DisablableUIRoot))]
public class ItemUI : MonoBehaviour {
    [SerializeField] private Image _MainImage;
    [SerializeField] private Image _BlockImage;

    private DisablableUIRoot _DisablableUI;

    private DisablableUIRoot DisablableUI {
        get {
            if (_DisablableUI == null) _DisablableUI = GetComponent<DisablableUIRoot>();
            return _DisablableUI;
        }
    }

    private Tooltip_Skill _Tooltip;

    public Tooltip_Skill Tooltip {
        get {
            if (_Tooltip == null) _Tooltip = GetComponentInChildren<Tooltip_Skill>(true);
            return _Tooltip;
        }
    }

    public void InitAll(IActivableItemDataSO source) {
        // Item's avatar
        _MainImage.sprite = source.avatar;

        // Item's tooltip window
        var descriptionDict = new SerializedDictionary<string, List<float_Q3>>(source.GenerateConcreteData_StringKey());
        Tooltip.Window.Init(
            avatar: source.avatar
          , skillName: source.itemName
          , mainText_Dynamic: new(source.description, descriptionDict)
          , details_Dynamic: new(source.details, descriptionDict)
          , cooldownTime: source.cooldownTime
          , activeCost: source.activeCost
          , maxLevel: source.maxLevel);
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

#region STACK

    [SerializeField] private TextMeshProUGUI _StackText;

    [HideInInspector] public int CurStack;

    public bool HaveStack {
        set => _StackText.gameObject.SetActive(value);
    }

#endregion

#region ACTIVE KEY

    [SerializeField] private TextMeshProUGUI _ActiveKeyText;

    private KeyControl _ActiveKey;

    public bool WasActiveThisFrame => _ActiveKey.wasPressedThisFrame;

    public void ChangeActiveKey(KeyControl activeKey) {
        _ActiveKey          = activeKey;
        _ActiveKeyText.text = activeKey.keyCode.ToString();
    }

#endregion
}