using NGDtuanh.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "All Tower Data", menuName = "Data/All Tower Data")]
public class AllTowerDataSO : ScriptableObject {
    public CovEnumMap<TowerId, TowerDataManaged> value;
}