using System.Collections.Generic;
using System.Linq;
using NGDtuanh.BubleAsset;
using NGDtuanh.BubleAsset.ShortCut;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

public struct MinionWaveData : IComponentData {
    public BlobAssetReference<Buble_Array_Array<MinionId>> _WaveLoopRef;
    public BlobAssetReference<BubleArray<MinionId>>        _WaveSuperRef;

    public ref Buble_Array_Array<MinionId> waveLoop  => ref _WaveLoopRef.Value;
    public ref BubleArray<MinionId>        waveSuper => ref _WaveSuperRef.Value;
}

public class MinionWaveAuthoring : MonoBehaviour {
    public CovEnumMap<MinionWaveType, List<MinionId>> waveTypes;
    public List<MinionWaveType>                       waveLoop;

    private class Baker : ExtendBaker<MinionWaveAuthoring> {
        public override void Bake(MinionWaveAuthoring authoring) {
            GetDynamicEntity(out var entity);

            var waveData = new MinionWaveData();

            authoring.waveLoop.Select(wave => authoring.waveTypes[wave]).ToList()
                .CreateBlobAssetReferenceInBaker(out waveData._WaveLoopRef, this, out _);

            authoring.waveTypes[MinionWaveType.WithSuper]
                .CreateBlobAssetReferenceInBaker(out waveData._WaveSuperRef, this, out _);

            AddComponent(entity, waveData);
        }
    }
}