using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct DamageOverTime : IBlobBuildable<DamageOverTime.Managed>, IBlobBuildableSelf<DamageOverTime> {
    public bool          enable;
    public ScalableFloat damage;
    public uint          periodTick;

    public void BuildBlob(ref BlobBuilder builder, Managed source) {
        enable = source.enable;
        damage.BuildBlob(ref builder, source.damage);
        periodTick = TickHelpers.CountTick(source.periodTime, GameSO.TickRate, TickHelpers.RoundMethod.Nearest);
    }

    public void BuildBlob(ref BlobBuilder builder, ref DamageOverTime source) {
        enable = source.enable;
        damage.BuildBlob(ref builder, ref source.damage);
        periodTick = source.periodTick;
    }

    public Final ComputeFinal(in Scaler.Metadata metadata) {
        if (!enable) return default;

        return new Final {
            enable     = enable
          , damage     = damage.GetScaledValue(metadata)
          , periodTick = periodTick
        };
    }

    public struct Final {
        public bool        enable;
        public float_Q3    damage;
        public uint        periodTick;
        public NetworkTick nextDamageTick;

        public readonly bool ItsTimeToDamage(in NetworkTick curTick) =>
            nextDamageTick.IsValid
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
         && curTick.IsNewerThan(nextDamageTick);

        public void UpdateNextDamageTick(in NetworkTick curTick) =>
            nextDamageTick = curTick.WithBonusTick(periodTick);

        public void StackWith(in Final target) {
            if (!enable) return;
            
            damage += target.damage;
        }

        public void Unstack(int oldStack, int newStack) {
            if (!enable) return;
            
            damage -= damage * (oldStack - newStack) / oldStack;
        }
    }

    [Serializable]
    public class Managed : IEnableable {
        [field: SerializeField] public override bool enable { get; set; }

        public ScalableFloat.Managed damage;
        public float                 periodTime;
    }
}