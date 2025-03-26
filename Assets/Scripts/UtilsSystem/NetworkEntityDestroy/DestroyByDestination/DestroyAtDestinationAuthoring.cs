using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct DestroyAtDestination : IComponentData {
    [GhostField(Quantization = 0)] public float3 destination;
}

public class DestroyAtDestinationAuthoring : MonoBehaviour {
    private class Baker : Baker<DestroyAtDestinationAuthoring> {
        public override void Bake(DestroyAtDestinationAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<DestroyAtDestination>(entity);
        }
    } 
}