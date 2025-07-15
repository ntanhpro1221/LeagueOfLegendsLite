using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemSkillUI : IItemUIWrapper {
    [field: SerializeField] public Tooltip_Skill Tooltip { get; private set; }

    public void InitAll(IActivableItemSO source) {
        if (source.HaveLevel) {
            Core.SetDisableFactor(ItemUIDisableFactor.NotEnoughLevel, true);

            // Init level points
            InitLevelPoints(source.maxLevel);
        }

        // Init item's avatar
        Core.Avatar.sprite = source.avatar;

        // Init tooltip window
        Tooltip.Window.Init(source);
    }

    public void UpdateAll(int newLevelPoints, int availablePoint) {
        UpdateLevelPoint(newLevelPoints);
        UpdateUpLevelBtn(availablePoint);
    }

    #region UP LEVEL BUTTON

    [SerializeField] private Button _UpLevelBtn;

    private void UpdateUpLevelBtn(int availablePoint) {
        _UpLevelBtn.gameObject.SetActive(availablePoint > 0 && _CurLevel < _LevelPoints.Count);
    }

    public void RegisterUpLevelListener(UnityAction callback) {
        _UpLevelBtn.onClick.AddListener(callback);
    }

    #endregion

    #region LEVEL POINT

    [SerializeField] private GameObject _SkillLevelPointPrefab;
    [SerializeField] private Transform  _LevelPointRoot;

    private int                 _CurLevel;

    private readonly List<IDisablableUI> _LevelPoints = new();

    private void UpdateLevelPoint(int newLevelPoints) {
        if (_CurLevel != newLevelPoints)
            Core.SetDisableFactor(ItemUIDisableFactor.NotEnoughLevel, newLevelPoints == 0);

        if (_CurLevel < newLevelPoints)
            for (; _CurLevel < newLevelPoints; ++_CurLevel)
                _LevelPoints[_CurLevel].OnEnable();
        else if (_CurLevel < newLevelPoints)
            for (; _CurLevel > newLevelPoints; --_CurLevel)
                _LevelPoints[_CurLevel - 1].OnDisable();
    }

    private void InitLevelPoints(int maxLevel) {
        if (_LevelPoints.Count != 0) {
            Debug.LogError($"NGDtuanh: you have called init level point more than one time in {name}");
        }

        _CurLevel = 0;

        for (int i = 1; i <= maxLevel; ++i) {
            var newPoint        = Instantiate(_SkillLevelPointPrefab, _LevelPointRoot);
            var newDisablableUI = newPoint.GetComponent<IDisablableUI>();
            _LevelPoints.Add(newDisablableUI);
            newDisablableUI.OnDisable();

            // Because of padding
            newPoint.transform.SetSiblingIndex(i);
        }
    }

    #endregion
}