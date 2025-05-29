using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSkillUI : MonoBehaviour {
    public ItemUI ItemUI { get; private set; }

    private void Awake() {
        ItemUI           = GetComponentInChildren<ItemUI>();
        _UpLevelDisabler = _UpLevelBtn.GetComponent<IDisablableUI>();
    }

    public void InitAll(IActivableItemDataSO source) {
        // Level points 
        InitLevelPoints(source.maxLevel);

        // parent ItemUI
        ItemUI.InitAll(source);
    }

    public void UpdateAll(int newLevelPoints, int availablePoint) {
        UpdateLevelPoint(newLevelPoints);
        UpdateUpLevelBtn(availablePoint);
    }

#region UP LEVEL BUTTON

    [SerializeField] private Button _UpLevelBtn;
    
    private IDisablableUI _UpLevelDisabler;

    private void UpdateUpLevelBtn(int availablePoint) {
        _UpLevelBtn.interactable = _CurLevel < _LevelPoints.Count;
        if (_UpLevelBtn.interactable)
            _UpLevelDisabler.OnEnable();
        else _UpLevelDisabler.OnDisable();

        _UpLevelBtn.gameObject.SetActive(availablePoint > 0);
    }

#endregion

#region LEVEL POINT

    [SerializeField] private GameObject _SkillLevelPointPrefab;
    [SerializeField] private Transform  _LevelPointRoot;

    private int                 _CurLevel;
    private List<IDisablableUI> _LevelPoints = new();

    private void UpdateLevelPoint(int newLevelPoints) {
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