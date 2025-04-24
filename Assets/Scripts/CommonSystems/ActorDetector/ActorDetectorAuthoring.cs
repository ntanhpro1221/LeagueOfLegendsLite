using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct ActorDetector : IComponentData {
    /// <summary>
    /// Must be replicated because:
    /// When server and client not spawn ActorDetector at the same tick,
    /// instance of ActorDetector will be created on the client (replicate from server) before the client spawns it.
    /// And in some ways, this replicated ActorDetector's data is not synced in some tick (server will correct it later).
    /// </summary>
    [GhostField] public Entity holder;

    public float3 tmpHolderPosition;

    public static implicit operator ActorDetector(Entity holder)
        => new() { holder = holder };
}

[RequireComponent(typeof(CapsuleCollider))]
public class ActorDetectorAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<ActorDetectorAuthoring> {
        public override void Bake(ActorDetectorAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent<ActorDetector>(entity);
        }
    }
}