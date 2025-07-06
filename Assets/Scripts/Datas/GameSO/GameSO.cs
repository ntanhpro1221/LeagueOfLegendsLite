using NGDtuanh.Collections;
using Unity.NetCode;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSO", menuName = "Data/GameSO")]
public class GameSO : ScriptableObject {
    #region RESOURCE HANDLE

    private const string RESOURCE_PATH = "GameSO";
    private const string ASSET_PATH    = "Assets/Scripts/Datas/GameSO/Resources/GameSO.asset";

    private static GameSO _Instance;

    private static GameSO _CachedInstance {
        get {
            if (_Instance == null) {
                #if UNITY_EDITOR
                _Instance = UnityEditor.AssetDatabase.LoadAssetAtPath<GameSO>(ASSET_PATH);
                #else
                _Instance = Resources.Load<GameSO>(RESOURCE_PATH);
                #endif
            }

            return _Instance;
        }
    }

    #endregion

    #region ALL SCRIPTABLE OBJECT

    [SerializeField] private NetCodeConfig        _NetConfig;
    [SerializeField] private RoomConnectionConfig _RoomConnectionConfig;
    [SerializeField] private AllChampionDataSO    _Champion;
    [SerializeField] private AllMinionDataSO      _Minion;
    [SerializeField] private AllMonsterDataSO     _Monster;
    [SerializeField] private AllItemDataSO        _Item;
    [SerializeField] private AllTowerDataSO       _Tower;
    [SerializeField] private AllEffectDataSO      _Effect;

    #endregion

    #region ALL ACCESSORS

    public static NetCodeConfig                               NetConfig             => _CachedInstance._NetConfig;
    public static RoomConnectionConfig                        RoomConnectionConfig  => _CachedInstance._RoomConnectionConfig;
    public static int                                         TickRate              => NetConfig.ClientServerTickRate.SimulationTickRate;
    public static CovEnumMap<ChampionId, ChampionDataManaged> Champ                 => _CachedInstance._Champion.value;
    public static CovEnumMap<BountyId, float_Q3>              ChampCommonInitBounty => _CachedInstance._Champion.commonInitBounty;
    public static CovEnumMap<MinionId, MinionDataManaged>     Minion                => _CachedInstance._Minion.value;
    public static CovEnumMap<MonsterId, MonsterDataManaged>   Monster               => _CachedInstance._Monster.value;
    public static CovEnumMap<ItemId, ItemDataManaged>         Item                  => _CachedInstance._Item.value;
    public static CovEnumMap<TowerId, TowerDataManaged>       Tower                 => _CachedInstance._Tower.value;
    public static CovEnumMap<EffectId, EffectData.Managed>    Effect                => _CachedInstance._Effect.value;

    #endregion
}