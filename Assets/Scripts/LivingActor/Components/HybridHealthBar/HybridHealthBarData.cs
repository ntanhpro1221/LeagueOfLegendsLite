using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct HybridHealthBarData : ICleanupComponentData {
    public float                         deltaY;
    public UnityObjectRef<RectTransform> transRef;
    public UnityObjectRef<HealthBarUI>   UIRef;
}