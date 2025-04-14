using Unity.Entities;
using UnityEngine;

public struct TurretTag : IComponentData { }

public class TurretTagAuthoring : MonoBehaviour {
    private class Baker : TagBaker<TurretTagAuthoring, TurretTag> { }
}