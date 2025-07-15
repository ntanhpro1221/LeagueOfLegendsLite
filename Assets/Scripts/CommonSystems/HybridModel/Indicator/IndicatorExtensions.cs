public static class IndicatorExtensions {
    public static ref IndicatorShower.Metadata WithNormalAttack(
        this ref IndicatorShower.Metadata metadata
      , bool                              showNormalAttack
      , float_Q3                          attackRange) {
        metadata.showNormalAttack = showNormalAttack;
        metadata.attackRange      = attackRange;
        return ref metadata;
    }

    public static ref IndicatorShower.Metadata WithTurretData(
        this ref IndicatorShower.Metadata metadata
      , float3_Q3                         ownChampPos
      , bool                              ownChampIsTarget
      , float3_Q3                         ownerPos) {
        metadata.ownChampPos      = ownChampPos;
        metadata.ownChampIsTarget = ownChampIsTarget;
        metadata.input.ownerPos   = ownerPos;
        return ref metadata;
    }

    public static ref IndicatorShower.Metadata WithActivableItem(
        this ref IndicatorShower.Metadata         metadata
      , SlotItemId                                itemKey
      , int                                       ownerLevel
      , int                                       selfLevel
      , in InputForActivableItemData              input
      , in Strum.ItemActiveCond.Fields<bool> condition) {
        metadata.itemKey    = itemKey;
        metadata.ownerLevel = ownerLevel;
        metadata.selfLevel  = selfLevel;
        metadata.input      = input;
        metadata.condition  = condition;
        return ref metadata;
    }

    public static ref IndicatorShower.Metadata WithoutActivableItem(this ref IndicatorShower.Metadata metadata) {
        metadata.itemKey = (SlotItemId)Strum.SlotItem.Count;
        return ref metadata;
    }

    public static bool IsWithoutItem(this in IndicatorShower.Metadata metadata) =>
        metadata.itemKey == (SlotItemId)Strum.SlotItem.Count;
}