using UnityEngine;

[CreateAssetMenu(fileName = "Simple Fixed Range", menuName = ASSET_PATH + "Simple Fixed Range")]
public class IndicatorSimpleFixedRange : IndicatorSingleStickyBase {
    [SerializeField] private float _Radius;

    public override void UpdateShower(Transform trans, in IndicatorShower.Metadata metadata, ref ActivableItemData itemData) {
        trans.localScale    = 2 * new Vector3(_Radius, 1, _Radius);
        trans.localPosition = Vector3.zero;
    }
}