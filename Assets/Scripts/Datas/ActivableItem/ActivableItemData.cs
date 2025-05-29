using System;
using NGDtuanh.BubleAsset;
using NGDtuanh.BubleAsset.ShortCut;
using Unity.Entities;

[Serializable]
public struct ActivableItemData : IBlobBuildable<IActivableItemDataSO>, IBlobBuildableSelf<ActivableItemData> {
    public ItemActivationCondition           activationCondition;
    public BubleArray<ItemCommonLeveledData> leveledData_Common;
    public Buble_Map_Array<int, float_Q3>    leveledData_Concrete;

    public void BuildBlob(ref BlobBuilder builder, IActivableItemDataSO source) {
        activationCondition = source.activationCondition;
        leveledData_Common.BuildBlob(ref builder, source.leveledData_Common);
        leveledData_Concrete.BuildBlob(ref builder, source.GenerateConcreteData_IntKey());
    }

    public void BuildBlob(ref BlobBuilder builder, ref ActivableItemData source) {
        activationCondition = source.activationCondition;
        leveledData_Common.BuildBlob(ref builder, ref source.leveledData_Common);
        leveledData_Concrete.BuildBlob(ref builder, ref source.leveledData_Concrete);
    }
}