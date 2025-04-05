using NGDtuanh.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "All Minion Data", menuName = "Data/All Minion Data")]
public class AllMinionDataSO : ScriptableObject {
    public CovEnumMap<MinionId, MinionDataManaged> value;
}