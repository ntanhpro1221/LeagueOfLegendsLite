using UnityEngine;

public abstract class IndicatorDynamicBase : IndicatorConcreteBase {
    public override void Enable(IndicatorShower components) {
        components.DynamicIndicator.sharedMaterial = _MainMate;
        components.DynamicIndicator.enabled = true;
    }

    public override void Disable(IndicatorShower components) {
        components.DynamicIndicator.enabled = false;
    }

    public override void UpdateShower(IndicatorShower components, in IndicatorShower.Metadata metadata, ref ActivableItemData itemData)
        => UpdateShower(components.DynamicIndicator.transform, metadata, ref itemData);

    public abstract void UpdateShower(
        Transform                    trans
      , in  IndicatorShower.Metadata metadata
      , ref ActivableItemData        itemData);
}