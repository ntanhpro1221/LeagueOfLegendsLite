using System.Collections.Generic;
using NGDtuanh.Collections;
using NGDtuanh.Singleton;
using UnityEngine;

public class MovementIndicatorPoolingManager : SceneSingleton<MovementIndicatorPoolingManager> {
    [field: SerializeField] public float arrowRollEnd { get; private set; } = -170;
    [field: SerializeField] public float ringScaleEnd { get; private set; } = 1;
    [field: SerializeField] public float duration     { get; private set; } = 1;

    [SerializeField] private EnumMap<MovementIndicatorType, GameObject> pattern;

    private EnumMap<MovementIndicatorType, Stack<MovementIndicator>> available;

    protected override void Awake() {
        base.Awake();

        foreach (var key in (available = new()).Keys) available[key] = new();
    }

    public static void Pool(Vector3 position, MovementIndicatorType type) {
        var available = Instance.available[type];
        if (available.Count == 0) {
            var newIndicator = Instantiate(
                    Instance.pattern[type]
                  , Instance.transform)
                .GetComponent<MovementIndicator>();
            available.Push(newIndicator);
            newIndicator.OnComplete += () => ImDone(newIndicator, type);
        }

        var indicator = available.Pop();
        indicator.Restart();
        indicator.gameObject.SetActive(true);
        indicator.transform.position = position;
    }

    public static void ImDone(MovementIndicator indicator, MovementIndicatorType type) {
        Instance.available[type].Push(indicator);
        indicator.gameObject.SetActive(false);
    }
}