using Unity.Entities;
using UnityEngine;

public struct NexusTag : IComponentData { }

public class NexusTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<NexusTagAuthoring, NexusTag> { }
}