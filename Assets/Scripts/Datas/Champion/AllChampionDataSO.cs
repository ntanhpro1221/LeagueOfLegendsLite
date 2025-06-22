using NGDtuanh.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "All Champion Data", menuName = "Data/All Champion Data")]
public class AllChampionDataSO : ScriptableObject {
    public CovEnumMap<BountyId, float_Q3>            commonInitBounty;
    public CovEnumMap<ChampionId, ChampionDataManaged> value;
}
