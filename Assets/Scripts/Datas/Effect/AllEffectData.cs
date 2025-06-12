using NGDtuanh.BubleAsset;
using Unity.Entities;

public struct AllEffectData : IComponentData {
    public BlobAssetReference<BubleEnMap<EffectId, EffectData, EffectData.Managed>> _Ref;

    public ref BubleEnMap<EffectId, EffectData, EffectData.Managed> Effects => ref _Ref.Value;
}