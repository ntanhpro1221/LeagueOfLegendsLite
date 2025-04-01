using NGDtuanh.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "All Item Data", menuName = "Data/All Item Data")]
public class AllItemDataSO : ScriptableObject {
    public CovEnumMap<ItemId, ItemDataManaged> value;
} 