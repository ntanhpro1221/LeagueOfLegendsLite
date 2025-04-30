using Pathfinding;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct ActiveObstacle : IComponentData, IEnableableComponent {
    public UnityObjectRef<NavmeshCut> _Ref;

    public NavmeshCut Obstacle {
        get => _Ref.Value;
        set => _Ref.Value = value;
    }
}

public class ActiveObstacleAuthoring : MonoBehaviour {
    private class Baker : DisabledTagBaker<ActiveObstacleAuthoring, ActiveObstacle> { }
}