using Unity.Entities;
using UnityEngine;

public struct YasuoTag : IComponentData { }

public class YasuoTagAuthoring : MonoBehaviour {
    public class Baker : TagBaker<YasuoTagAuthoring, YasuoTag> { }
}