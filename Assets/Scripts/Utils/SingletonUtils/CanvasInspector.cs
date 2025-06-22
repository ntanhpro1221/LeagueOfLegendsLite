using NGDtuanh.Singleton;
using UnityEngine;

public class CanvasInspector : SceneSingleton<CanvasInspector> {
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