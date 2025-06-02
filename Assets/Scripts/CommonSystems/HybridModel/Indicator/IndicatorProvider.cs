using System.Collections.Generic;
using NGDtuanh.Singleton;
using UnityEngine;

public class IndicatorProvider : SceneSingleton<IndicatorProvider> {
    [SerializeField] private GameObject _IndicatorPrefab;

    private readonly Dictionary<IndicatorShower, Material> _TurretWarningMates = new();
    public static    Dictionary<IndicatorShower, Material> TurretWarningMates => Instance._TurretWarningMates;

    public MeshRenderer SpawnNewIndicator(Transform root) {
        var indicator = Instantiate(_IndicatorPrefab, root);
        return indicator.GetComponent<MeshRenderer>();
    }
}