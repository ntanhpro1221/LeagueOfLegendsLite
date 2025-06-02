using UnityEngine;

[CreateAssetMenu(fileName = "Ashe W Indicator", menuName = ASSET_PATH + "Ashe W Indicator")]
public class IndicatorMultiLine_AsheW : IndicatorMultiLineBase {
    protected override int GetLineAmount(in IndicatorShower.Metadata metadata, ref ActivableItemData itemData) =>
        (int)itemData.concreteProp.Value[(int)AsheSkill_W.ConcreteProperty.arrowAmount][metadata.selfLevel];
}