using Unity.Entities;
using UnityEngine;

public struct RedBramblebackTag : IComponentData { }

public class RedBramblebackTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<RedBramblebackTagAuthoring, RedBramblebackTag> { }
}