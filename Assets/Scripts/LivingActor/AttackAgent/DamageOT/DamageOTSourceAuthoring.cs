using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct DamageOTSource : IComponentData {
    [GhostField] public int      damageOT;
    [GhostField] public float_Q3 interval;
    [GhostField] public float_Q3 timeResidual;

    /// <summary>
    ///Just to use for temporary calculation in <see cref="HandleDamageFromOTSourceSystem"/> (don't synchronize this)
    /// </summary>
    public int tmpTotalDamage;
}

[RequireComponent(typeof(TeamTypeAuthoring))]
public class DamageOTSourceAuthoring : MonoBehaviour {
    public int      damageOT;
    public int      interval;
    public AreaType areaType;

    private class Baker : Baker<DamageOTSourceAuthoring> {
        public override void Bake(DamageOTSourceAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DamageOTSource {
                damageOT = authoring.damageOT
              , interval = authoring.interval
            });

            switch (authoring.areaType) {
                case AreaType.Targeted: AddComponent<DamageTargetData>(entity); break;
                case AreaType.Area:     AddComponent<DamageAreaTag>(entity); break;
                default:                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum AreaType {
        Targeted
      , Area
    }
}