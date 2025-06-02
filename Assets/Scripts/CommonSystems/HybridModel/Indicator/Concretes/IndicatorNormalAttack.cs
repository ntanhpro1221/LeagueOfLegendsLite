using UnityEngine;

[CreateAssetMenu(fileName = "Normal Attack", menuName = ASSET_PATH + "Normal Attack")]
public class IndicatorNormalAttack : IndicatorConcreteBase {
    public override void Enable(IndicatorShower components) {
        components.NormalAttack.sharedMaterial = _MainMate;
        components.NormalAttack.enabled        = true;
    }

    public override void Disable(IndicatorShower components) {
        components.NormalAttack.enabled = false;
    }

    public override void UpdateShower(
        IndicatorShower              components
      , in  IndicatorShower.Metadata metadata
      , ref ActivableItemData        itemData) {
        components.NormalAttack.transform.localScale = 2 * new Vector3(metadata.attackRange, 1, metadata.attackRange);
    }
}