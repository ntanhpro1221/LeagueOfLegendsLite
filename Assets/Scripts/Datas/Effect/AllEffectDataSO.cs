using NGDtuanh.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "All Effect Data", menuName = "Data/All Effect Data")]
public class AllEffectDataSO : ScriptableObject {
    public CovEnumMap<EffectId, EffectDataManaged> value;
}