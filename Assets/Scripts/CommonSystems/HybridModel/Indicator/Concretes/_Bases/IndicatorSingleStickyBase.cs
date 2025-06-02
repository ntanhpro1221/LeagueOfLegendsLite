using UnityEngine;

public abstract class IndicatorSingleStickyBase : IndicatorConcreteBase {
    public override void Enable(IndicatorShower components) {
        components.StickyIndicator.sharedMaterial = _MainMate;
        components.StickyIndicator.enabled = true;
    }

    public override void Disable(IndicatorShower components) {
        components.StickyIndicator.enabled = false;
    }

    public override void UpdateShower(IndicatorShower components, in IndicatorShower.Metadata metadata, ref ActivableItemData itemData)
        => UpdateShower(components.StickyIndicator.transform, metadata, ref itemData);

    public abstract void UpdateShower(
        Transform                    trans
      , in  IndicatorShower.Metadata metadata
      , ref ActivableItemData        itemData);
}