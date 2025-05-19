using Unity.Entities;
using UnityEngine;

public struct KrugTag : IComponentData { }

public class KrugTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<KrugTagAuthoring, KrugTag> { }
}
