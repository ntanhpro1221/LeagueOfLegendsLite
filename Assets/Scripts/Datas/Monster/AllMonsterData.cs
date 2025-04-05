using System;
using NGDtuanh.BubleAsset;
using Unity.Entities;

[Serializable]
public struct AllMonsterData : IComponentData {
    public BlobAssetReference<BubleEnMap<MonsterId, MonsterData, MonsterDataManaged>> _Ref;

    public ref BubleEnMap<MonsterId, MonsterData, MonsterDataManaged> Monsters => ref _Ref.Value;
}