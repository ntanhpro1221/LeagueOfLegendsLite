using Unity.Entities;

using BigMonster_Ref = Unity.Entities.BlobAssetReference<InitTransformData_BigMonster>;
using Champion_Ref = Unity.Entities.BlobAssetReference<InitTransformData_Champion>;
using MinionPath_Ref = Unity.Entities.BlobAssetReference<InitTransformData_MinionPath>;
using Monster_Ref = Unity.Entities.BlobAssetReference<InitTransformData_Monster>;
using ScuttlePath_Ref = Unity.Entities.BlobAssetReference<InitTransformData_ScuttlePath>;
using Tower_Ref = Unity.Entities.BlobAssetReference<InitTransformData_Tower>;

using BigMonster = BlobAssetExtend.BlobHashMap<BlobAssetExtend.EquatableEnum<TransformKeys.BigMonster>, InitTransformData>;
using Champion = Unity.Entities.BlobArray<InitTransformData>;
using MinionPath = BlobAssetExtend.BlobHashMap<BlobAssetExtend.EquatableEnum<TransformKeys.MinionPath>, Unity.Entities.BlobArray<InitTransformData>>;
using Monster = BlobAssetExtend.BlobHashMap<BlobAssetExtend.EquatableEnum<TransformKeys.Monster>, InitTransformData>;
using ScuttlePath = BlobAssetExtend.BlobHashMap<BlobAssetExtend.EquatableEnum<TransformKeys.ScuttlePath>, Unity.Entities.BlobArray<InitTransformData>>;
using Tower = BlobAssetExtend.BlobHashMap<BlobAssetExtend.EquatableEnum<TransformKeys.Tower>, InitTransformData>;

public struct InitTransformData_Reference : IComponentData {
    public BigMonster_Ref  _BigMonster;
    public Champion_Ref    _Champion_Blue;
    public Champion_Ref    _Champion_Red;
    public MinionPath_Ref  _MinionPath_Blue;
    public MinionPath_Ref  _MinionPath_Red;
    public Monster_Ref     _Monster_Blue;
    public Monster_Ref     _Monster_Red;
    public ScuttlePath_Ref _ScuttlePath;
    public Tower_Ref       _Tower_Blue;
    public Tower_Ref       _Tower_Red;

    public ref BigMonster  BigMonster      => ref _BigMonster.Value.value;
    public ref Champion    Champion_Blue   => ref _Champion_Blue.Value.value;
    public ref Champion    Champion_Red    => ref _Champion_Red.Value.value;
    public ref MinionPath  MinionPath_Blue => ref _MinionPath_Blue.Value.value;
    public ref MinionPath  MinionPath_Red  => ref _MinionPath_Red.Value.value;
    public ref Monster     Monster_Blue    => ref _Monster_Blue.Value.value;
    public ref Monster     Monster_Red     => ref _Monster_Red.Value.value;
    public ref ScuttlePath ScuttlePath     => ref _ScuttlePath.Value.value;
    public ref Tower       Tower_Blue      => ref _Tower_Blue.Value.value;
    public ref Tower       Tower_Red       => ref _Tower_Red.Value.value;
}