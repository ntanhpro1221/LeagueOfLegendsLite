using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct ItemData : IBlobBuildable<ItemDataManaged>, IBlobBuildableSelf<ItemData> {
    public ActivableItemData                       common;
    public BubleArray<StatBuffs.ElementUnscalable> buffs;
    public BubleArray<ItemId>                      recipe;
    public uint                                    lifeTick;
    public Settings                                settings;

    public void BuildBlob(ref BlobBuilder builder, ItemDataManaged source) {
        common.BuildBlob(ref builder, source.common);
        buffs.BuildBlob(ref builder, source.buffs);
        recipe.BuildBlob(ref builder, source.recipe);
        lifeTick = TickHelpers.CountTick(source.lifeTime, GameSO.TickRate, TickHelpers.RoundMethod.Nearest);
        settings = source.settings;
    }

    public void BuildBlob(ref BlobBuilder builder, ref ItemData source) {
        common.BuildBlob(ref builder, ref source.common);
        buffs.BuildBlob(ref builder, ref source.buffs);
        recipe.BuildBlob(ref builder, ref source.recipe);
        lifeTick = source.lifeTick;
        settings = source.settings;
    }

    [Serializable]
    public struct Settings {
        /// <summary>
        /// Moment of lifeTime that will perform real action.<br/>
        /// Value in range [0, 1].
        /// </summary>
        [Range(0, 1)]
        public float triggerPoint;

        public bool followActivateDir;

        public float_Q3 cost, sell;
    }
}