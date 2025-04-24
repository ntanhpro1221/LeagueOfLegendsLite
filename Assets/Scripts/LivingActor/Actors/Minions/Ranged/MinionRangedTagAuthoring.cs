using Unity.Entities;
using UnityEngine;

public struct MinionRangedTag : IComponentData { }

public class MinionRangedTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<MinionRangedTagAuthoring, MinionRangedTag> { }
}