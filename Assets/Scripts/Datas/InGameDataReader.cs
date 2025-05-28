using NGDtuanh.Collections;
using NGDtuanh.Singleton;
using UnityEngine;

public class InGameDataReader : SceneSingleton<InGameDataReader> {
    [SerializeField] private AllChampionDataSO _Champion;
    [SerializeField] private AllMinionDataSO   _Minion;
    [SerializeField] private AllMonsterDataSO  _Monster;
    [SerializeField] private AllItemDataSO     _Item;
    [SerializeField] private AllTowerDataSO    _Tower;

    public static CovEnumMap<ChampionId, ChampionDataManaged> Champion => Instance._Champion.value;
    public static CovEnumMap<MinionId, MinionDataManaged>     Minion   => Instance._Minion.value;
    public static CovEnumMap<MonsterId, MonsterDataManaged>   Monster  => Instance._Monster.value;
    public static CovEnumMap<ItemId, ItemDataManaged>         Item     => Instance._Item.value;
    public static CovEnumMap<TowerId, TowerDataManaged>       Tower    => Instance._Tower.value;
}