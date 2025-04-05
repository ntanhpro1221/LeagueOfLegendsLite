using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct AllMinionData : IComponentData {
    public BlobAssetReference<BubleEnMap<MinionId, MinionData, MinionDataManaged>> _Ref;

    public ref BubleEnMap<MinionId, MinionData, MinionDataManaged> Minions => ref _Ref.Value;
}