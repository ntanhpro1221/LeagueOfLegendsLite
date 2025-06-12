using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public struct EffectFixedLife : IBlobBuildable<EffectFixedLife.Managed>, IBlobBuildableSelf<EffectFixedLife> {
    public bool enable;

    public ScalableFloat lifeTick;

    public void BuildBlob(ref BlobBuilder builder, Managed       source) {
        enable = source.enable;

        lifeTick.BuildBlob(ref builder, source.lifeTime);
        lifeTick.origin = (int)TickHelpers.CountTick(lifeTick.origin, GameSO.TickRate, TickHelpers.RoundMethod.Nearest);
    }

    public void BuildBlob(ref BlobBuilder builder, ref EffectFixedLife source) {
        enable = source.enable;

        lifeTick.BuildBlob(ref builder, ref source.lifeTick);
    }

    public uint GetLifeTick(in Scaler.Metadata metadata) => enable
        ? (uint)lifeTick.GetScaledValue(metadata)
        : metadata.customLifeTick;

    [Serializable]
    // ReSharper disable once MemberHidesStaticFromOuterClass
    public class Managed : IEnableable {
        [field: SerializeField] public override bool enable { get; set; }

        public ScalableFloat.Managed lifeTime;
    }
}