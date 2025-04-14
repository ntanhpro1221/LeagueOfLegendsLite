using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct Selectable : IComponentData, IEnableableComponent { }

public class SelectableAuthoring : MonoBehaviour {
    private class Baker : TagBaker<SelectableAuthoring, Selectable> { }
}