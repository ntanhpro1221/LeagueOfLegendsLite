using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GhostEnabledBit]
public struct StatsData : IComponentData, IEnableableComponent {
    [GhostField] public Strum.Stats.Fields<float_Q3> data;
}

[GhostEnabledBit]
public struct StatsData_Raw : IComponentData, IEnableableComponent {
    [GhostField] public Strum.Stats.Fields<float_Q3> data;
}

[GhostEnabledBit]
public struct StatsData_RawPerLevel : IComponentData, IEnableableComponent {
    [GhostField] public Strum.Stats.Fields<float_Q3> data;
}

public class StatsAuthoring : MonoBehaviour {
    public bool haveLevel;

    private class Baker : ExtendBaker<StatsAuthoring> {
        public override void Bake(StatsAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponentDisabled<StatsData>(entity);
            AddComponentDisabled<StatsData_Raw>(entity);

            if (authoring.haveLevel)
                AddComponentDisabled<StatsData_RawPerLevel>(entity);
        }
    }
}