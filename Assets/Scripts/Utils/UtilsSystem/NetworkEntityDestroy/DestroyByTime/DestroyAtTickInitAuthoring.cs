using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Serialization;

public struct DestroyAtTickInitData : IComponentData {
    [GhostField(Quantization = 0)] public float lifeTime;
}

[RequireComponent(typeof(NetworkDestroyableAuthoring))]
public class DestroyAtTickInitAuthoring : MonoBehaviour {
    public float lifeTime;

    private class Baker : Baker<DestroyAtTickInitAuthoring> {
        public override void Bake(DestroyAtTickInitAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DestroyAtTickInitData {
                lifeTime = authoring.lifeTime
            });
        }
    }
}