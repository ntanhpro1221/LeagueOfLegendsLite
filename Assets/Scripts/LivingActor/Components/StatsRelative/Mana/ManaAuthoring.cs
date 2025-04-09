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

    private class Baker : ExtendBaker<ManaAuthoring> {
        public override void Bake(ManaAuthoring authoring) {
            GetDynamicEntity(out var entity);

            if (authoring.useThisDefaultValue)
                AddComponent(entity, new ManaData { value = authoring.value });
            else
                AddComponentDisabled<ManaData>(entity);
        }
    }
}