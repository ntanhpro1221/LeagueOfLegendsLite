using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct ManaData : IComponentData, IEnableableComponent {
    [GhostField] public float_Q3 value; 
}

[RequireComponent(typeof(StatsAuthoring))]
public class ManaAuthoring : MonoBehaviour {
    private class Baker : Baker<ManaAuthoring> {
        public override void Bake(ManaAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            this.AddComponentDisabled<ManaData>(entity);
        }
    }
}