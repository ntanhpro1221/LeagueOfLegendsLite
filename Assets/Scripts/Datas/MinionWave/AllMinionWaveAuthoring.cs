using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

public struct AllMinionWaveData : IComponentData {
    public BlobAssetReference<BubleEnMap<MinionWaveType, MinionWaveData, MinionWaveDataManaged>> _Ref;

    public ref BubleEnMap<MinionWaveType, MinionWaveData, MinionWaveDataManaged> Data => ref _Ref.Value;

    public float intervalBetweenTwoMinions;
}

public class AllMinionWaveAuthoring : MonoBehaviour {
    public CovEnumMap<MinionWaveType, MinionWaveDataManaged> waves;
    public float                                             intervalBetweenTwoMinions;

    private class Baker : ExtendBaker<AllMinionWaveAuthoring> {
        public override void Bake(AllMinionWaveAuthoring authoring) {
            GetDynamicEntity(out var entity);

            var data = new AllMinionWaveData();
            authoring.waves.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            data.intervalBetweenTwoMinions = authoring.intervalBetweenTwoMinions;
            AddComponent(entity, data);
        }
    }
}