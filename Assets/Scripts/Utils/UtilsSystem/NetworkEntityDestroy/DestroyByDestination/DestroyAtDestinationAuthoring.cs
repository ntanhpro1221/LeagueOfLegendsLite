using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct DestroyAtDestination : IComponentData {
    [GhostField(Quantization = 0)] public float3 destination;
}

[RequireComponent(typeof(NetworkDestroyableAuthoring))]
public class DestroyAtDestinationAuthoring : MonoBehaviour {
    public Vector3 destination;

    private class Baker : Baker<DestroyAtDestinationAuthoring> {
        public override void Bake(DestroyAtDestinationAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DestroyAtDestination {
                destination = authoring.destination
            });
        }
    }
}