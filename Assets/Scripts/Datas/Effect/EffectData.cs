using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public struct EffectData : IBlobBuildable<EffectData.Managed>, IBlobBuildableSelf<EffectData> {
    public EffectStackingBehaviour stackingBehaviour;

    public DamageOverTime  damageOT;
    public StatBuffs       statBuffs;
    public CC.Disable      ccDisable;
    public CC.Control      ccControl;
    public EffectFixedLife fixedLife;

    public void BuildBlob(ref BlobBuilder builder, Managed source) {
        stackingBehaviour = source.stackingBehaviour;

        damageOT.BuildBlob(ref builder, source.damageOverTime);
        statBuffs.BuildBlob(ref builder, source.statBuffs);
        ccDisable.BuildBlob(ref builder, source.ccDisable);
        ccControl.BuildBlob(ref builder, source.ccControl);
        fixedLife.BuildBlob(ref builder, source.fixedLife);
    }

    public void BuildBlob(ref BlobBuilder builder, ref EffectData source) {
        stackingBehaviour = source.stackingBehaviour;

        damageOT.BuildBlob(ref builder, ref source.damageOT);
        statBuffs.BuildBlob(ref builder, ref source.statBuffs);
        ccDisable.BuildBlob(ref builder, ref source.ccDisable);
        ccControl.BuildBlob(ref builder, ref source.ccControl);
        fixedLife.BuildBlob(ref builder, ref source.fixedLife);
    }

    [Serializable]
    public class Managed {
        public string   name;
        public BarData  barData;
        public BodyData bodyData;
        public IconData iconData;

        [TextArea(1, 3)]
        public string description;

        public EffectStackingBehaviour stackingBehaviour;
        public DamageOverTime.Managed  damageOverTime;
        public StatBuffs.Managed       statBuffs;
        public CC.Disable.Managed      ccDisable;
        public CC.Control.Managed      ccControl;
        public EffectFixedLife.Managed fixedLife;

        [Serializable]
        public class BarData : IEnableable {
            [field: SerializeField] public override bool enable { get; set; }

            public Sprite avatar;
            public Color  outlineColor;
            public bool   showStack;
            public bool   showTimer;
        }

        [Serializable]
        public class BodyData : IEnableable {
            [field: SerializeField] public override bool enable { get; set; }

            public EffectBodyId bodyId;
        }

        [Serializable]
        public class IconData : IEnableable {
            [field: SerializeField] public override bool enable { get; set; }

            public Sprite icon;
        }
    }
}