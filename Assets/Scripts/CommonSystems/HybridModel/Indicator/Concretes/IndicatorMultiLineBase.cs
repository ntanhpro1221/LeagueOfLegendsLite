using UnityEngine;

public abstract class IndicatorMultiLineBase : IndicatorConcreteBase {
    [SerializeField] private int   _LineDisDegree;
    [SerializeField] private Vector2 _Size;

    private bool _FirstUpdateAfterEnable;
    
    protected abstract int GetLineAmount(in IndicatorShower.Metadata metadata, ref ActivableItemData itemData);

    public override void Enable(IndicatorShower components) {
        components.MultiIndicatorRoot.gameObject.SetActive(true);
        _FirstUpdateAfterEnable = true;
    }

    public override void Disable(IndicatorShower components) {
        components.MultiIndicatorRoot.gameObject.SetActive(false);
    }

    public override void UpdateShower(
        IndicatorShower              components
      , in  IndicatorShower.Metadata metadata
      , ref ActivableItemData        itemData) {

        int amount = GetLineAmount(metadata, ref itemData);
        if (_FirstUpdateAfterEnable) {
            _FirstUpdateAfterEnable = false;

            components.EnsureMultiIndicatorSize(amount);

            for (int i = 0; i < amount; ++i) {
                components.MultiIndicator[i].sharedMaterial = _MainMate;
                components.MultiIndicator[i].enabled        = true;
            }

            for (int i = amount; i < components.MultiIndicator.Count; ++i)
                components.MultiIndicator[i].enabled = false;
        }

        var delta        = Quaternion.Euler(0, _LineDisDegree * (-1), 0);
        var curDirection = Quaternion.Euler(0, _LineDisDegree * ((float)amount - 1) / 2, 0) * metadata.input.direction.Full;

        for (int i = 0; i < amount; ++i, curDirection = delta * curDirection)
            IndicatorSimpleFixedLine.UpdateLine(
                components.MultiIndicator[i].transform
              , _Size, curDirection);
    }
}