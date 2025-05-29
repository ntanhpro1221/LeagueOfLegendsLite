using NGDtuanh.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DataSOReader", menuName = "Data/DataSOReader")]
public class DataSOReader : ScriptableObject {
    [SerializeField] private AllChampionDataSO _Champion;
    [SerializeField] private AllMinionDataSO   _Minion;
    [SerializeField] private AllMonsterDataSO  _Monster;
    [SerializeField] private AllItemDataSO     _Item;
    [SerializeField] private AllTowerDataSO    _Tower;

    public CovEnumMap<ChampionId, ChampionDataManaged> Champ   => _Champion.value;
    public CovEnumMap<MinionId, MinionDataManaged>     Minion  => _Minion.value;
    public CovEnumMap<MonsterId, MonsterDataManaged>   Monster => _Monster.value;
    public CovEnumMap<ItemId, ItemDataManaged>         Item    => _Item.value;
    public CovEnumMap<TowerId, TowerDataManaged>       Tower   => _Tower.value;
}