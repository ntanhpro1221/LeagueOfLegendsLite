using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct HealthData : IComponentData, IEnableableComponent {
    [GhostField] public float_Q3 value;
}

[RequireComponent(typeof(StatsAuthoring))]
public class HealthAuthoring : MonoBehaviour {
    private class Baker : Baker<HealthAuthoring> {
        public override void Bake(HealthAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            this.AddComponentDisabled<HealthData>(entity);
        }
    }
}