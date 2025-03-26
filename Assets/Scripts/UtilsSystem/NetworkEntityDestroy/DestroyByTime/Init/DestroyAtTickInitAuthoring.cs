using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct DestroyAtTickInitData : IComponentData {
    [GhostField(Quantization = 0)] public float delayBeforeDestroy;
}

public class DestroyAtTickInitAuthoring : MonoBehaviour {
    public float delayBeforeDestroy;

    private class Baker : Baker<DestroyAtTickInitAuthoring> {
        public override void Bake(DestroyAtTickInitAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DestroyAtTickInitData {
                delayBeforeDestroy = authoring.delayBeforeDestroy
            });
        }
    }
}