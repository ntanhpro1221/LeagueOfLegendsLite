using Unity.Entities;
using UnityEngine;

public struct ScuttleTag : IComponentData { }

public class ScuttleTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<ScuttleTagAuthoring, ScuttleTag> { }
}