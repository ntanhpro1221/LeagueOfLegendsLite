using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct AllTowerData : IComponentData {
    public BlobAssetReference<BubleEnMap<TowerId, TowerData, TowerDataManaged>> _Ref;

    public ref BubleEnMap<TowerId, TowerData, TowerDataManaged> Towers => ref _Ref.Value;
}