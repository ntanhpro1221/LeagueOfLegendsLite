using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

public struct SkillData : ICleanupComponentData, IDisposable {
    public BlobAssetReference<ActivableItemData>                                   _PassiveRef;
    public BlobAssetReference<BubleArray<ActivableItemData, IActivableItemDataSO>> _SkillsRef;

    public ref ActivableItemData                                   Passive => ref _PassiveRef.Value;
    public ref BubleArray<ActivableItemData, IActivableItemDataSO> Skills  => ref _SkillsRef.Value;

    public void CreateBlobAssetReference(ref ChampionData source) {
        source.passive.CreateBlobAssetReference(out _PassiveRef);
        source.skills.CreateBlobAssetReference(out _SkillsRef);
    }

    public void Dispose() {
        _PassiveRef.Dispose();
        _SkillsRef.Dispose();
    }
}