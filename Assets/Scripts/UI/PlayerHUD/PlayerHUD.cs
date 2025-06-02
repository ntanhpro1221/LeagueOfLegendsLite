using NGDtuanh.Singleton;
using UnityEngine;

[RequireComponent(
    typeof(DeadHandler_OwnChamp)
  , typeof(PlayerActivableItemUI))]
public class PlayerHUD : SceneSingleton<PlayerHUD> {
    [field: SerializeField] public StatsUI Stats { get; private set; }

    public PlayerActivableItemUI ActivableItems { get; private set; }
    public DeadHandler_OwnChamp  DeadHandler    { get; private set; }
    public HealthBarUI           HealthBar      { get; private set; }

    protected override void OnTouched() {
        base.OnTouched();

        ActivableItems = GetComponent<PlayerActivableItemUI>();
        DeadHandler    = GetComponent<DeadHandler_OwnChamp>();
        HealthBar      = GetComponentInChildren<HealthBarUI>();
    }
}