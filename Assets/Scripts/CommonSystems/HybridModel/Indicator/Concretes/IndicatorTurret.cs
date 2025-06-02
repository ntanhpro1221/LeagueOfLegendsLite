using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Turret", menuName = ASSET_PATH + "Turret")]
public class IndicatorTurret : IndicatorNormalAttack {
    [SerializeField] private Material _WarningMate;
    [SerializeField] private float    _MinWarningRatio, _MaxWarningRatio;

    public override void Enable(IndicatorShower components) {
        base.Enable(components);

        if (!IndicatorProvider.TurretWarningMates.ContainsKey(components))
            IndicatorProvider.TurretWarningMates.Add(components, Instantiate(_WarningMate));
    }

    public override void UpdateShower(
        IndicatorShower              components
      , in  IndicatorShower.Metadata metadata
      , ref ActivableItemData        itemData) {
        base.UpdateShower(components, metadata, ref itemData);

        if (metadata.ownChampIsTarget) components.NormalAttack.sharedMaterial = _MainMate;
        else {
            components.NormalAttack.sharedMaterial = IndicatorProvider.TurretWarningMates[components];
            float distanceToChamp = GameHelpers.DistanceXZ(metadata.input.ownerPos, metadata.ownChampPos);
            var   color           = IndicatorProvider.TurretWarningMates[components].color;
            color.a = 1 - Mathf.Clamp01(
                (distanceToChamp / metadata.attackRange - _MinWarningRatio)
              / (_MaxWarningRatio                       - _MinWarningRatio));
            IndicatorProvider.TurretWarningMates[components].color = color;
        }
    }
}