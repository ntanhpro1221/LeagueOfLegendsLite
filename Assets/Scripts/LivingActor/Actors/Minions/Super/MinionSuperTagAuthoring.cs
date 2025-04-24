using Unity.Entities;
using UnityEngine;

public struct MinionSuperTag : IComponentData { }

public class MinionSuperTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<MinionSuperTagAuthoring, MinionSuperTag> { }
}