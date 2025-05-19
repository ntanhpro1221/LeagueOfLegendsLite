using Unity.Entities;
using UnityEngine;

public struct RaptorTag : IComponentData { }

public class RaptorTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<RaptorTagAuthoring, RaptorTag> { }
}
