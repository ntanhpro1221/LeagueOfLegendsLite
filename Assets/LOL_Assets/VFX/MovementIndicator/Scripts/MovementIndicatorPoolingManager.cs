using System.Collections.Generic;
using NGDtuanh.Singleton;
using Unity.Mathematics;
using UnityEngine;

public class MovementIndicatorPoolingManager : SceneSingleton<MovementIndicatorPoolingManager> {
    [field: SerializeField] public float arrowRollEnd { get; private set; } = -170;
    [field: SerializeField] public float ringScaleEnd { get; private set; } = 1;
    [field: SerializeField] public float duration     { get; private set; } = 1;

    [SerializeField] private GameObject pattern;

    private Stack<MovementIndicator> availableIndicator = new();

    public static void Pool(Vector3 position) {
        var stack = Instance.availableIndicator;
        if (stack.Count == 0)
            stack.Push(Instantiate(
                    Instance.pattern
                  , Instance.transform)
                .GetComponent<MovementIndicator>());

        var indicator = stack.Pop();
        indicator.Restart();
        indicator.gameObject.SetActive(true);
        indicator.transform.position = position;
    }

    public static void ImDone(MovementIndicator indicator) {
        Instance.availableIndicator.Push(indicator);
        indicator.gameObject.SetActive(false);
    }
}