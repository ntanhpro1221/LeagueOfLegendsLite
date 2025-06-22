using NGDtuanh.Collections;
using NGDtuanh.Singleton;
using UnityEngine;

public class TooltipWindowHolder : SceneSingleton<TooltipWindowHolder> {
    [SerializeField] private EnumMap<TooltipWindowType, ITooltipWindow> _Prefabs;

    public TWindow GetWindow<TWindow>() where TWindow : ITooltipWindow =>
        Instantiate(
            _Prefabs[typeof(TWindow).GetWindowType()]
          , transform
        ).GetComponent<TWindow>();
}