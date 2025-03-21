using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "All Champion Data", menuName = "Data/All Champion Data")]
public class AllChampionDataSO : ScriptableObject {
    [FormerlySerializedAs("champions")] public AllChampionDataManaged value;
}
