using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct ManaData : IComponentData, IEnableableComponent {
    [GhostField] public float_Q3 value; 
}

[RequireComponent(typeof(StatsAuthoring))]
public class ManaAuthoring : MonoBehaviour {
    public float_Q3 value;
    public bool     useThisDefaultValue;

    private class Baker : Baker<ManaAuthoring> {
        public override void Bake(ManaAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            if (authoring.useThisDefaultValue)
                AddComponent(entity, new ManaData { value = authoring.value });
            else
                this.AddComponentDisabled<ManaData>(entity);
        }
    }
}