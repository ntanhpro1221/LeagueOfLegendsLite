using NGDtuanh.Singleton;
using UnityEngine;

public class TeamStatusHolderUI : SceneSingleton<TeamStatusHolderUI> {
    [SerializeField] private GameObject _ItemPrefab;
    [SerializeField] private Transform  _AllyTeam;
    [SerializeField] private Transform  _EnemyTeam;

    public TeamStatusItemUI SpawnItem(bool isAlly) =>
        Instantiate(
                _ItemPrefab
              , isAlly ? _AllyTeam : _EnemyTeam)
            .GetComponent<TeamStatusItemUI>();
}
