using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct BuffBuffer : IBufferElementData {
    [GhostField] public float_Q3 add;
    [GhostField] public float_Q3 mul;

    public void Add(BuffApplyType applyType, float_Q3 value) {
        switch (applyType) {
            case BuffApplyType.Add: add += value; break;
            case BuffApplyType.Mul: mul += value; break;
        }
    }

    public void Remove(BuffApplyType applyType, float_Q3 value)
        => Add(applyType, -value);

    public readonly float_Q3 ApplyTo(in StatsBuffer source) => (source.value + add) * (mul + 1);
}

[Serializable]
public struct IncomingBuffBuffer : IBufferElementData {
    [GhostField] public BuffApplyType    applyType;
    [GhostField] public float_Q3         value;
    [GhostField] public StatsType        statsType;
    [GhostField] public BuffDurationType durationType;
    [GhostField] public uint             durationTick;

    public static IncomingBuffBuffer Construct_AddPersistent(
        BuffApplyType applyType
      , in float_Q3   value
      , StatsType     statsType) => new() {
        applyType    = applyType
      , value        = value
      , statsType    = statsType
      , durationType = BuffDurationType.Persistent
    };

    public static IncomingBuffBuffer Construct_RemovePersistent(
        BuffApplyType applyType
      , in float_Q3   value
      , StatsType     statsType) => Construct_AddPersistent(
        applyType
      , -value
      , statsType);

    public static IncomingBuffBuffer Construct_AddTemp(
        BuffApplyType applyType
      , float_Q3      value
      , StatsType     statsType
      , uint          durationTick) => new() {
        applyType    = applyType
      , value        = value
      , statsType    = statsType
      , durationType = BuffDurationType.Temp
      , durationTick = durationTick
    };
}

public struct IncomingExpiringBuffBuffer : IBufferElementData {
    [GhostField] public BuffApplyType applyType;
    [GhostField] public float_Q3      value;
    [GhostField] public StatsType     statsType;
    [GhostField] public NetworkTick   expireAtTick;

    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
    public readonly bool IsExpired(in NetworkTick curTick) => curTick.IsNewerThan(expireAtTick);

    public static IncomingExpiringBuffBuffer Construct(
        in IncomingBuffBuffer buff
      , in NetworkTick        curTick) => new() {
        applyType    = buff.applyType
      , value        = buff.value
      , statsType    = buff.statsType
      , expireAtTick = curTick.WithBonusTick(buff.durationTick)
    };
}

public class BuffAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<BuffAuthoring> {
        public override void Bake(BuffAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddCleanBuffer<BuffBuffer>(entity, EnumCount.Stats);
            AddBuffer<IncomingBuffBuffer>(entity);
            AddBuffer<IncomingExpiringBuffBuffer>(entity);
        }
    }
}