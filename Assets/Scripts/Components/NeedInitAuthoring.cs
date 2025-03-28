using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct NeedInitTag : IComponentData { }

public class NeedInitAuthoring : MonoBehaviour {
    private class Baker : TagBaker<NeedInitAuthoring, NeedInitTag> { }
}