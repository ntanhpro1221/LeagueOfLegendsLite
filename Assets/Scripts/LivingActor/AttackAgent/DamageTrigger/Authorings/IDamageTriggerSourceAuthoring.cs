using System;
using System.Collections.Generic;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public interface IDamageTriggerType : IComponentData { }

public struct DamageTriggerSource : IComponentData {
    [GhostField] public float_Q3  damage;
    [GhostField] public Entity    source;
    [GhostField] public float3_Q3 sourcePos;

    [GhostField] public Scaler.Metadata.Personal sourceScaler;

    public static class Type {
        public struct Targeted : IDamageTriggerType { }

        public struct ShotBlockable : IDamageTriggerType { }

        public struct ShotNonBlockable : IDamageTriggerType { }
    }

    /// <summary>
    /// This is not blob asset, <see cref="IBlobBuildable{TSource}"/> just for convenient.
    /// </summary>
    public struct EffectBuffer : IBufferElementData {
        [GhostField] public EffectId id;
        [GhostField] public uint     customLifeTick;

        [Serializable]
        public class Managed {
            public EffectId id;
            public float    customLifeTime;

            public EffectBuffer ToUnmanaged() => new() {
                id             = id
              , customLifeTick = TickHelpers.CountTick(customLifeTime, GameSO.TickRate, TickHelpers.RoundMethod.Nearest)
            };
        }
    }
}

[RequireComponent(typeof(NetworkDestroyableAuthoring))]
public abstract class IDamageTriggerSourceAuthoring<TTypeTag> : MonoBehaviour where TTypeTag : struct, IDamageTriggerType {
    public float_Q3 damage;

    public List<DamageTriggerSource.EffectBuffer.Managed> effects;

    protected Entity BakeTriggerSourceBase(IBaker baker) {
        baker.GetDynamicEntity(out var entity);

        baker.AddComponent(entity, new DamageTriggerSource { damage = damage });
        baker.AddComponent<TTypeTag>(entity);

        var effectBuffer = baker.AddBuffer<DamageTriggerSource.EffectBuffer>(entity);
        foreach (var effectManaged in effects) effectBuffer.Add(effectManaged.ToUnmanaged());

        return entity;
    }
}