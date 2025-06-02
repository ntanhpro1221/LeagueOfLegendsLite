using UnityEngine;

public abstract class IndicatorStickyAndDynamicBase : IndicatorSingleStickyBase {
    [field: SerializeField] protected Material _DynamicMate { get; private set; }

    public override void Enable(IndicatorShower components) {
        base.Enable(components);
        components.DynamicIndicator.sharedMaterial = _DynamicMate;
        components.DynamicIndicator.enabled = true;
    }

    public override void Disable(IndicatorShower components) {
        base.Disable(components);
        components.DynamicIndicator.enabled = false;
    }

    public override void UpdateShower(IndicatorShower components, in IndicatorShower.Metadata metadata, ref ActivableItemData itemData)
        => UpdateShower(components.StickyIndicator.transform, components.DynamicIndicator.transform, metadata, ref itemData);

    public abstract void UpdateShower(
        Transform                    transSticky
      , Transform                    transDynamic
      , in  IndicatorShower.Metadata metadata
      , ref ActivableItemData        itemData);
}