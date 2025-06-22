using System;
using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public interface IHaveStatsManaged {
    CovEnumMap<StatId, float_Q3> Stats { get; }
}

public interface IHaveStatsPerLevelManaged : IHaveStatsManaged {
    CovEnumMap<StatId, float_Q3> StatsPerLevel { get; }
}

public struct StatsData : IComponentData {
    [GhostField] public Strum.Stats.Fields<float_Q3> data;

#region UTIL FUNCTIONS

    public void CopyFromRaw(in Raw raw) {
        ref var rawData = ref raw._ref.Value;
        foreach (var index in Strum.Stats.Indexes)
            data[index] = rawData[index];
    }

    public void ApplyLevel(in RawPerLevel rawPerLevel, in LevelData level) {
        ref var rawPerLevelData = ref rawPerLevel._ref.Value;
        foreach (var index in Strum.Stats.Indexes)
            data[index] += rawPerLevelData[index] * (level.curLevel - 1);
    }

    public void ApplyBuffs(in StatBuffs.Receiver buffs) {
        ref readonly var buffsData = ref buffs.buffs;

        foreach (var index in Strum.Stats.Indexes)
            buffsData[index].ApplyTo(ref data.ValueRW(index));
    }

#endregion

#region COMPONENTS

    public struct Raw : IComponentData {
        public BlobAssetReference<BubleEnMap<StatId, float_Q3>> _ref;

        public static Raw ConstructInBaker(CovEnumMap<StatId, float_Q3> source, IBaker baker) {
            Raw result = default;
            source.CreateBlobAssetReferenceInBaker(out result._ref, baker, out _);
            return result;
        }
    }

    public struct RawPerLevel : IComponentData {
        public BlobAssetReference<BubleEnMap<StatId, float_Q3>> _ref;

        public static RawPerLevel ConstructInBaker(CovEnumMap<StatId, float_Q3> source, IBaker baker) {
            RawPerLevel result = default;
            source.CreateBlobAssetReferenceInBaker(out result._ref, baker, out _);
            return result;
        }
    }

#endregion
}

[RequireComponent(typeof(IRaceTag))]
public class StatsAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<StatsAuthoring> {
        public override void Bake(StatsAuthoring authoring) {
            if (ActorAuthoringHelpers.IsBaseRace(authoring)) return;
            
            GetDynamicEntity(out var entity);

            var data = ActorAuthoringHelpers.ExtractDataFromTag(authoring);

            if (data is not IHaveStatsManaged statsSource)
                throw new Exception(
                    $"NGDtuanh: {authoring.name}'s data must have stats");

            var raw = StatsData.Raw.ConstructInBaker(statsSource.Stats, this);
            AddComponent(entity, raw);

            // Try to add stats per level data
            if (data is IHaveStatsPerLevelManaged statsPerLevelSource) {
                AddComponent(entity, StatsData.RawPerLevel.ConstructInBaker(statsPerLevelSource.StatsPerLevel, this));

                if (authoring.GetComponent<LevelAuthoring>() == null)
                    throw new Exception(
                        $"NGDtuanh: {authoring.name}'s data have level data but not have {nameof(LevelAuthoring)} component");
            }

            // Add stats data
            var stats = default(StatsData);
            stats.CopyFromRaw(raw);
            AddComponent(entity, stats);
        }
    }
}