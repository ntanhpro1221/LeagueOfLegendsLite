using Unity.Entities;
using UnityEngine;

public struct InhibitorTag : IComponentData { }

public class InhibitorTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<InhibitorTagAuthoring, InhibitorTag> { }
}