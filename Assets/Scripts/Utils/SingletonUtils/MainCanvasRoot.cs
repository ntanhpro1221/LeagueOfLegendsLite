using NGDtuanh.Singleton;
using UnityEngine;

public class MainCanvasRoot : SceneSingleton<MainCanvasRoot> {
    [field: SerializeField]
    public RectTransform HealthBarRoot { get; private set; }
    
    public RectTransform RectTrans     { get; private set; }

    protected override void OnTouched() {
        base.OnTouched();
        RectTrans = transform as RectTransform;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        RectTrans = null;
    }
}