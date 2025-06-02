using UnityEngine;

[CreateAssetMenu(fileName = "Splittable", menuName = ASSET_PATH + "Splittable")]
public class IndicatorSplittable : IndicatorConcreteBase {
    [SerializeField] private Vector2 _Size;

    [SerializeField, Space, Header("STICKY")]
    private Material _SplitStickyMate;

    [SerializeField] private Vector2 _SplitStickySize;

    [SerializeField, Space, Header("DYNAMIC")]
    private Material _DynamicMate;

    [SerializeField] private Vector2 _DynamicSize;

    [SerializeField, Space, Header("SPLIT CONFIG")]
    private float _SplitDistance;

    private void VisibleAll(IndicatorShower components, bool visible) {
        components.StickyIndicator.enabled      = visible;
        components.DynamicIndicator.enabled     = visible;
        components.SplitStickyIndicator.enabled = visible;
    }

    private void VisibleSplit(IndicatorShower components, bool split) {
        components.StickyIndicator.enabled      = !split;
        components.DynamicIndicator.enabled     = split;
        components.SplitStickyIndicator.enabled = split;
    }
    
    public override void Enable(IndicatorShower components) {
        components.StickyIndicator.sharedMaterial      = _MainMate;
        components.DynamicIndicator.sharedMaterial     = _DynamicMate;
        components.SplitStickyIndicator.sharedMaterial = _SplitStickyMate;
    }

    public override void Disable(IndicatorShower components) {
        VisibleAll(components, false);
    }

    public override void UpdateShower(
        IndicatorShower              components
      , in  IndicatorShower.Metadata metadata
      , ref ActivableItemData        itemData) {
        Vector3 direction = metadata.input.direction.Full;
        float   distance  = GameHelpers.DistanceXZ(metadata.input.ownerPos, metadata.input.ground);
        if (_SplitDistance < distance) {
            VisibleSplit(components, true);
            IndicatorSimpleFixedLine.UpdateLine(components.SplitStickyIndicator.transform, _SplitStickySize, direction);
            IndicatorSimpleFixedLine.UpdateLine(components.DynamicIndicator.transform,     _DynamicSize,     direction, distance);
        } else {
            VisibleSplit(components, false);
            IndicatorSimpleFixedLine.UpdateLine(components.StickyIndicator.transform, _Size, direction);
        }
    }
}