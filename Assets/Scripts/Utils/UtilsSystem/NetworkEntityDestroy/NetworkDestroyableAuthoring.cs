using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct NetworkDestroyedTag : IComponentData, IEnableableComponent { }

public class NetworkDestroyableAuthoring : MonoBehaviour {
    private class Baker : DisabledTagBaker<NetworkDestroyableAuthoring, NetworkDestroyedTag> { }
}