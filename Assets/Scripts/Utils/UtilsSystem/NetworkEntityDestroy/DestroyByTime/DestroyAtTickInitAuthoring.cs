using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Serialization;

[GhostEnabledBit]
public struct DestroyAfterPeriod : IComponentData, IEnableableComponent {
    [GhostField] public float_Q3 lifeTime;
}

[RequireComponent(typeof(NetworkDestroyableAuthoring))]
public class DestroyAtTickInitAuthoring : MonoBehaviour {
    public float lifeTime;

    private class Baker : Baker<DestroyAtTickInitAuthoring> {
        public override void Bake(DestroyAtTickInitAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DestroyAfterPeriod {
                lifeTime = authoring.lifeTime.Quantizate3()
            });
        }
    }
}