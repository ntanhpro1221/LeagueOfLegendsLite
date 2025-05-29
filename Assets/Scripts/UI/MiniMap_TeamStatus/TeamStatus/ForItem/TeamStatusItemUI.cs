using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DeadHandler_TeamStatus))]
public class TeamStatusItemUI : MonoBehaviour {
    [SerializeField] private DataSOReader _SOReader;
    [SerializeField] private Image        _Avatar;

    public HealthBarUI            HealthBarUI { get; private set; }
    public DeadHandler_TeamStatus DeadHandler { get; private set; }

    private void Awake() {
        HealthBarUI = GetComponentInChildren<HealthBarUI>(true);
        DeadHandler = GetComponent<DeadHandler_TeamStatus>();
    }

    public void SetAvatar(ChampionId champId) {
        _Avatar.sprite = _SOReader.Champ[champId].avatar;
    }
}