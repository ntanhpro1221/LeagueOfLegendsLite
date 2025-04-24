using Unity.Entities;
using UnityEngine;

public struct MinionSiegeTag : IComponentData { }

public class MinionSiegeTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<MinionSiegeTagAuthoring, MinionSiegeTag> { }
}