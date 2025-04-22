using NGDtuanh.Singleton;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(FunnelModifier))]
[RequireComponent(typeof(RaycastModifier))]
public class PathModifierHub : SceneSingleton<PathModifierHub> {
    public static FunnelModifier  Funnel  => Instance._Funnel;
    public static RaycastModifier Raycast => Instance._Raycast;

    private FunnelModifier  _Funnel;
    private RaycastModifier _Raycast;

    protected override void Awake() {
        base.Awake();

        _Funnel  = GetComponent<FunnelModifier>();
        _Raycast = GetComponent<RaycastModifier>();
    }
}