using System.Linq;
using NGDtuanh.BubleAsset;
using NGDtuanh.BubleAsset.ShortCut;
using Unity.Entities;

public struct ActivableItemData : IBlobBuildable<IActivableItemSO>, IBlobBuildableSelf<ActivableItemData> {
    public ItemActiveSettings             activeSettings;
    public ItemActiveCondition            activeCondition;
    public bool                           haveLevel;
    public int                            maxLevel;
    public BubleArray<uint>               cooldownTick;
    public BubleArray<ItemActiveCost>     activeCost;
    public Buble_Map_Array<int, float_Q3> concreteProp;

    public void BuildBlob(ref BlobBuilder builder, IActivableItemSO source) {
        activeSettings  = source.activeSettings;
        activeCondition = source.activeCondition;
        haveLevel       = source.haveLevel;
        maxLevel        = source.maxLevel;
        cooldownTick.BuildBlob(ref builder, source.cooldownTime.Select(time =>
            TickHelpers.CountTick(time, GameSO.TickRate, TickHelpers.RoundMethod.Nearest)).ToList());
        activeCost.BuildBlob(ref builder, source.activeCost);
        concreteProp.BuildBlob(ref builder, source.GenerateConcreteData_IntKey());
    }

    public void BuildBlob(ref BlobBuilder builder, ref ActivableItemData source) {
        activeSettings  = source.activeSettings;
        activeCondition = source.activeCondition;
        haveLevel       = source.haveLevel;
        maxLevel        = source.maxLevel;
        cooldownTick.BuildBlob(ref builder, ref source.cooldownTick);
        activeCost.BuildBlob(ref builder, ref source.activeCost);
        concreteProp.BuildBlob(ref builder, ref source.concreteProp);
    }

    public int CalcLevelIndex(int level) => haveLevel
        ? level - 1
        // When this item don't need level, its level is always 1.
        : 0;
}