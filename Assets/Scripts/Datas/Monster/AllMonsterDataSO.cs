using NGDtuanh.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "All Monster Data", menuName = "Data/All Monster Data")]
public class AllMonsterDataSO : ScriptableObject {
    public CovEnumMap<MonsterId, MonsterDataManaged> value;
}