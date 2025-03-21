using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "All Item Data", menuName = "Data/All Item Data")]
public class AllItemDataSO : ScriptableObject {
    public AllItemDataManaged value;
}