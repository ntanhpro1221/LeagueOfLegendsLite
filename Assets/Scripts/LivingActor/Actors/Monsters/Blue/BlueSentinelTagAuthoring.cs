using Unity.Entities;
using UnityEngine;

public struct BlueSentinelTag : IComponentData { }

public class BlueSentinelTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<BlueSentinelTagAuthoring, BlueSentinelTag> { }
}