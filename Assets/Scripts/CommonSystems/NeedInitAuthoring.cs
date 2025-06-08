using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct NeedInitTag : IComponentData, IEnableableComponent { }

public class NeedInitAuthoring : MonoBehaviour {
    private class Baker : TagBaker<NeedInitAuthoring, NeedInitTag> { }
}