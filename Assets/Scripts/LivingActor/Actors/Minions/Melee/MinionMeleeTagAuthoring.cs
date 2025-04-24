using Unity.Entities;
using UnityEngine;

public struct MinionMeleeTag : IComponentData { }

public class MinionMeleeTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<MinionMeleeTagAuthoring, MinionMeleeTag> { }
}