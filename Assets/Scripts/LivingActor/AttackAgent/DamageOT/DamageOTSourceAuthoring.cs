using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct DamageOTSource : IComponentData {
    [GhostField(Quantization = 0)] public float damageOT;
    [GhostField(Quantization = 0)] public float interval;
    [GhostField(Quantization = 0)] public float timeResidual;

    /// <summary>
    ///Just to use for temporary calculation in <see cref="HandleDamageFromOTSourceSystem"/> (don't synchronize this)
    /// </summary>
    public float tmpTotalDamage;
}

[RequireComponent(typeof(TeamTypeAuthoring))]
public class DamageOTSourceAuthoring : MonoBehaviour {
    public float    damageOT;
    public float    interval;
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