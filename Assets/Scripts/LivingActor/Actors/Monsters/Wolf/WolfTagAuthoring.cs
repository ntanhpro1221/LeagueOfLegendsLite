using Unity.Entities;
using UnityEngine;

public struct WolfTag : IComponentData { }

public class WolfTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<WolfTagAuthoring, WolfTag> { }
}
