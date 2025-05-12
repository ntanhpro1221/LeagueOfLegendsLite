using Unity.Entities;
using UnityEngine;

public struct NeedInitCachedPathAndLineCast : IComponentData { }

public class CachePathAndLineCastAuthoring : MonoBehaviour {
    private class Baker : TagBaker<CachePathAndLineCastAuthoring, NeedInitCachedPathAndLineCast> { }
}