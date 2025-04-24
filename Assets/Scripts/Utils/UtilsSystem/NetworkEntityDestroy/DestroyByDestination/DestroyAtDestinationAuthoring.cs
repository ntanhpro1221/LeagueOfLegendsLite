using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct DestroyAtDestination : IComponentData, IEnableableComponent {
    [GhostField] public float3_Q3 destination;
}

[RequireComponent(typeof(NetworkDestroyableAuthoring))]
public class DestroyAtDestinationAuthoring : MonoBehaviour {
    public new bool    enabled;
    public     Vector3 destination;

    private class Baker : ExtendBaker<DestroyAtDestinationAuthoring> {
        public override void Bake(DestroyAtDestinationAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new DestroyAtDestination {
                destination = authoring.destination.Quantizate3()
            });

            SetComponentEnabled<DestroyAtDestination>(entity, authoring.enabled);
        }
    }
}