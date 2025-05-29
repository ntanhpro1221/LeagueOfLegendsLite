using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[RequireComponent(typeof(DisablableUIRoot))]
public class ItemUI : MonoBehaviour {
    [SerializeField] private Image _MainImage;

    private DisablableUIRoot _DisablableUI;
    private Tooltip_Skill    _Tooltip; 

    private void Awake() {
        _DisablableUI = GetComponent<DisablableUIRoot>();
        _Tooltip      = GetComponentInChildren<Tooltip_Skill>(true);
    }

    public void InitAll(IActivableItemDataSO source) {
        // Item's avatar
        _MainImage.sprite = source.avatar;

        // Item's tooltip window
        var descriptionDict = new SerializedDictionary<string, List<float_Q3>>(source.GenerateConcreteData_StringKey());
        _Tooltip.Window.Init(
            avatar: source.avatar
          , skillName: source.itemName
          , mainText_Dynamic: new(source.description, descriptionDict)
          , details_Dynamic: new(source.details, descriptionDict)
          , leveledData_Common: source.leveledData_Common
          , maxLevel: source.maxLevel);
    }

    private bool _IsCooldownVisible {
        set {
            _CooldownImage.gameObject.SetActive(value);
            _CooldownText.gameObject.SetActive(value);
        }
    }

    public bool IsInteractable {
        set {
            if (value) _DisablableUI.EnableAll();
            else _DisablableUI.DisableAll();
        }
    }

#region COOLDOWN

    [SerializeField] private Image           _CooldownImage;
    [SerializeField] private TextMeshProUGUI _CooldownText;

    private float _TotalCooldown;

    public void StartCooldown(float totalCooldown) {
        IsInteractable = false;

        _IsCooldownVisible = true;

        UpdateCooldown(_TotalCooldown = totalCooldown);
    }

    public void UpdateCooldown(float curCooldown) {
        _CooldownText.text        = ((int)curCooldown).ToString();
        _CooldownImage.fillAmount = curCooldown / _TotalCooldown;
    }

    public void DoneCooldown() {
        IsInteractable = true;

        _IsCooldownVisible = false;
    }

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