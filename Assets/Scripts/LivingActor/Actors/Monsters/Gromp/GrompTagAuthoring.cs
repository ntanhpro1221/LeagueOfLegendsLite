using Unity.Entities;
using UnityEngine;

public struct GrompTag : IComponentData { }

public class GrompTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<GrompTagAuthoring, GrompTag> { }
}