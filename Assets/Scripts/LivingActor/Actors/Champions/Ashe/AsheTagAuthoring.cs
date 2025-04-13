using Unity.Entities;
using UnityEngine;

public struct AsheTag : IComponentData { }

public class AsheTagAuthoring : MonoBehaviour {
    public class Baker : TagBaker<AsheTagAuthoring, AsheTag> { }
}