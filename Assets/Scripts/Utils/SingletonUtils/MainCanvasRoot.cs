using NGDtuanh.Singleton;
using UnityEngine;

public class MainCanvasRoot : SceneSingleton<MainCanvasRoot> {
    public RectTransform RectTrans;

    protected override void OnTouched() {
        base.OnTouched();
        RectTrans = transform as RectTransform;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        RectTrans = null;
    }
}