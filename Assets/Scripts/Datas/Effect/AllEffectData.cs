using NGDtuanh.BubleAsset;
using Unity.Entities;

public struct AllEffectData : IComponentData {
    public BlobAssetReference<BubleEnMap<EffectId, EffectData, EffectDataManaged>> _Ref;

    public ref BubleEnMap<EffectId, EffectData, EffectDataManaged> Effects => ref _Ref.Value;
}