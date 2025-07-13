using NGDtuanh.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Number Popup Styles", menuName = "Data/Number Popup Styles")]
public class NumberPopupStyleSO : ScriptableObject {
    public EnumMap<NumberPopup.Id, NumberPopup.Style> Data;
}