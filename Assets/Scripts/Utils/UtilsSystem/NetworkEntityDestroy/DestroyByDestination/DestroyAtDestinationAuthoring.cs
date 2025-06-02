using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Serialization;

[GhostEnabledBit]
public struct DestroyAtDestination : IComponentData, IEnableableComponent {
    [GhostField] public float3_Q3 destination;
}

public struct DestroyAtDesSettings : IComponentData {
    [GhostField] public bool useY;
}

[RequireComponent(typeof(NetworkDestroyableAuthoring))]
public class DestroyAtDestinationAuthoring : MonoBehaviour {
    public new bool    enabled;
    public     bool    useY;
    public     Vector3 destination;

    private class Baker : ExtendBaker<DestroyAtDestinationAuthoring> {
        public override void Bake(DestroyAtDestinationAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new DestroyAtDestination {
                destination = authoring.destination.Quantizate3()
            });

            AddComponent(entity, new DestroyAtDesSettings {
                useY = authoring.useY
            });

            SetComponentEnabled<DestroyAtDestination>(entity, authoring.enabled);
        }
    }
}