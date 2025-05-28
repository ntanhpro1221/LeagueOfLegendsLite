using NGDtuanh.Singleton;
using Unity.Mathematics;
using UnityEngine;

public class MiniMapUI : SceneSingleton<MiniMapUI> {
    [SerializeField] private GameObject    _FakeActorPrefab_Champion;
    [SerializeField] private RectTransform _ActorRoot;

    [SerializeField] private GameObject    _RealPoint;
    [SerializeField] private RectTransform _FakeBlue;
    [SerializeField] private RectTransform _FakeRed;

    private float   _RatioToFakeMap;
    private Vector2 _RealRoot;
    private float   _RealDis;

    protected override void Awake() {
        base.Awake();

        var realPoint = _RealPoint.GetComponent<MiniMapRealPoint>();
        _RealDis = Vector2.Distance(_RealRoot = realPoint.Blue.position.XZ(), realPoint.Red.position.XZ());
    } 

    public void UpdatePosInMap(RectTransform trans, Vector2 realPos) =>
        trans.anchoredPosition = _FakeBlue.anchoredPosition + (realPos - _RealRoot) / _RealDis
          * Vector2.Distance(_FakeBlue.anchoredPosition, _FakeRed.anchoredPosition);

    public FakeActorUI_Champion SpawnChamp(ChampionId id, bool isAlly) => Instantiate(
            _FakeActorPrefab_Champion
          , _ActorRoot)
        .GetComponent<FakeActorUI_Champion>()
        .Init(id, isAlly);
}