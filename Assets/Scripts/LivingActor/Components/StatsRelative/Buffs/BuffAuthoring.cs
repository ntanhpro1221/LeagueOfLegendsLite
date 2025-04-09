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
}

[Serializable]
public struct IncomingBuffBuffer : IBufferElementData {
    [GhostField] public BuffApplyType    applyType;
    [GhostField] public float_Q3         value;
    [GhostField] public StatsType        statsType;
    [GhostField] public BuffDurationType durationType;
    [GhostField] public float_Q3         duration;

    public static IncomingBuffBuffer Construct_AddPersistent(
        BuffApplyType applyType
      , in float_Q3      value
      , StatsType     statsType)
        => new() {
            applyType    = applyType
          , value        = value
          , statsType    = statsType
          , durationType = BuffDurationType.Persistent
        };

    public static IncomingBuffBuffer Construct_RemovePersistent(
        BuffApplyType applyType
      , in float_Q3      value
      , StatsType     statsType)
        => Construct_AddPersistent(
            applyType
          , -value
          , statsType);

    public static IncomingBuffBuffer Construct_AddTemp(
        BuffApplyType applyType
      , float_Q3      value
      , StatsType     statsType
      , float_Q3      duration)
        => new() {
            applyType    = applyType
          , value        = value
          , statsType    = statsType
          , durationType = BuffDurationType.Temp
          , duration     = duration
        };
}

public struct UpcomingExpiringBuffBuffer : IBufferElementData, IComparable<UpcomingExpiringBuffBuffer> {
    [GhostField] public BuffApplyType applyType;
    [GhostField] public float_Q3      value;
    [GhostField] public StatsType     statsType;
    [GhostField] public NetworkTick   expireAtTick;
    
    public int CompareTo(UpcomingExpiringBuffBuffer other) {
        if (expireAtTick.Equals(other.expireAtTick)) return 0;

        return expireAtTick.IsNewerThan(other.expireAtTick) ? -1 : 1;
    }

    public bool IsExpired(NetworkTick curTick) => curTick.IsNewerThan(expireAtTick);

    public static UpcomingExpiringBuffBuffer Construct(in IncomingBuffBuffer buff, in NetworkTick curTick, int tickRate) {
        var expireTick = curTick;
        expireTick.Add((uint)(buff.duration * tickRate));
        return new() {
            applyType    = buff.applyType
          , value        = buff.value
          , statsType    = buff.statsType
          , expireAtTick = expireTick
        };
    }
}

public class BuffAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<BuffAuthoring> {
        public override void Bake(BuffAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddCleanBuffer<BuffBuffer>(entity, Enum.GetValues(typeof(StatsType)).Length);
            AddBuffer<IncomingBuffBuffer>(entity);
            AddBuffer<UpcomingExpiringBuffBuffer>(entity);
        }
    }
}