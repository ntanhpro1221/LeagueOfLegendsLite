using UnityEngine;

[CreateAssetMenu(fileName = "Simple Fixed Line", menuName = ASSET_PATH + "Simple Fixed Line")]
public class IndicatorSimpleFixedLine : IndicatorSingleStickyBase {
    [SerializeField] private Vector2 _Size;

    public override void UpdateShower(Transform trans, in IndicatorShower.Metadata metadata, ref ActivableItemData itemData) {
        UpdateLine(trans, _Size, metadata.input.direction.Full);
    }

    public static void UpdateLine(Transform trans, Vector2 size, Vector3 dir, float offset = 0f) {
        trans.localScale    = new Vector3(size.x, 1, size.y);
        trans.localPosition = (offset + size.y / 2) * (trans.forward = dir).normalized;
    }
}