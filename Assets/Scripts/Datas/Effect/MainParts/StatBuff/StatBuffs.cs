using System;
using System.Collections.Generic;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct StatBuffs : IBlobBuildable<StatBuffs.Managed>, IBlobBuildableSelf<StatBuffs> {
    public bool enable;

    public BubleArray<Element, Element.Managed> buffs;

    public void BuildBlob(ref BlobBuilder builder, Managed source) {
        enable = source.enable;
        buffs.BuildBlob(ref builder, source.buffs);
    }

    public void BuildBlob(ref BlobBuilder builder, ref StatBuffs source) {
        enable = source.enable;
        buffs.BuildBlob(ref builder, ref source.buffs);
    }

    public Final ComputeFinal(in Scaler.Metadata metadata) {
        if (!enable) return default;

        return new Final {
            enable = enable
          , buffs  = ComputeBuff(metadata)
        };
    }

    private Strum.Stats.Fields<Strum.StatBuff.Fields<float_Q3>> ComputeBuff(in Scaler.Metadata metadata) {
        Strum.Stats.Fields<Strum.StatBuff.Fields<float_Q3>> result = default;
        for (int i = 0; i < buffs.Count; ++i) {
            ref var buff = ref buffs[i];
            result.ValueRW(buff.statId).ValueRW(buff.applyType)
                += buff.value.GetScaledValue(metadata);
        }

        return result;
    }

    public struct Element : IBlobBuildable<Element.Managed>, IBlobBuildableSelf<Element> {
        public StatBuffId    applyType;
        public ScalableFloat value;
        public StatId        statId;

        public void BuildBlob(ref BlobBuilder builder, Managed source) {
            applyType = source.applyType;
            value.BuildBlob(ref builder, source.value);
            statId = source.statId;
        }

        public void BuildBlob(ref BlobBuilder builder, ref Element source) {
            applyType = source.applyType;
            value.BuildBlob(ref builder, ref source.value);
            statId = source.statId;
        }

        [Serializable]
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public class Managed {
            public StatBuffId            applyType;
            public ScalableFloat.Managed value;
            public StatId                statId;
        }
    }

    [Serializable]
    public struct ElementUnscalable {
        public StatBuffId applyType;
        public float_Q3   value;
        public StatId     statId;
    }

    public struct Final {
        public bool enable;

        public Strum.Stats.Fields<Strum.StatBuff.Fields<float_Q3>> buffs;

        public void StackWith(in Final target) {
            if (!enable) return;

            target.buffs.ApplyTo(ref buffs, 1);
        }

        public void Unstack(int oldStack, int newStack) {
            if (!enable) return;

            var tmpBuffs = buffs;
            tmpBuffs.ApplyTo(ref buffs, -(oldStack - newStack) / oldStack);
        }
    }

    public struct Receiver : IComponentData {
        [GhostField] public Strum.Stats.Fields<Strum.StatBuff.Fields<float_Q3>> buffs;

        public void Add(in Final final) {
            if (!final.enable) return;

            final.buffs.ApplyTo(ref buffs, 1);
        }

        public void Add(ref BubleArray<ElementUnscalable> source) {
            source.ApplyTo(ref buffs, 1);
        }

        public void Remove(in Final final) {
            if (!final.enable) return;

            final.buffs.ApplyTo(ref buffs, -1);
        }

        public void Remove(ref BubleArray<ElementUnscalable> source) {
            source.ApplyTo(ref buffs, -1);
        }
    }

    [Serializable]
    public class Managed : IEnableable {
        [field: SerializeField] public override bool enable { get; set; }

        public List<Element.Managed> buffs;
    }
}

public static class StatBuffExtensions {
    public static void ApplyTo(this in Strum.StatBuff.Fields<float_Q3> buff, ref float_Q3 target) =>
        target = (target + buff.Add) * (1 + buff.Mul);

    public static void ApplyTo(
        this in Strum.Stats.Fields<Strum.StatBuff.Fields<float_Q3>> buffs
      , ref     Strum.Stats.Fields<Strum.StatBuff.Fields<float_Q3>> targets
      , float_Q3                                                    mul) {
        foreach (var statId in Strum.Stats.Indexes) {
            ref readonly var buff   = ref buffs.ValueRO(statId);
            ref var          target = ref targets.ValueRW(statId);
            foreach (var applyId in Strum.StatBuff.Indexes)
                target[applyId] += buff[applyId] * mul;
        }
    }

    public static void ApplyTo(
        this ref BubleArray<StatBuffs.ElementUnscalable>             source
      , ref      Strum.Stats.Fields<Strum.StatBuff.Fields<float_Q3>> targets
      , float_Q3                                                     mul) {
        for (int i = 0; i < source.Count; ++i)
            targets.ValueRW(source[i].statId).ValueRW(source[i].applyType) += source[i].value * mul;
    }
}