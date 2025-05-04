using Unity.Entities;
using UnityEngine;

public struct DummyTag : IComponentData { }

public class DummyAuthoring : MonoBehaviour {
    private class Baker : TagBaker<DummyAuthoring, DummyTag> { }
}