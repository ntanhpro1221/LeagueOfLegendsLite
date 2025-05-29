using NGDtuanh.Singleton;
using UnityEngine;
using UnityEngine.UI;

public class TeamStatusHolderUI : SceneSingleton<TeamStatusHolderUI> {
    [SerializeField] private GameObject    _ItemPrefab;
    [SerializeField] private RectTransform _AllyTeam;
    [SerializeField] private RectTransform _EnemyTeam;

    public TeamStatusItemUI SpawnItem(bool isAlly) => Instantiate(
            _ItemPrefab
          , isAlly ? _AllyTeam : _EnemyTeam)
        .GetComponent<TeamStatusItemUI>();
}