using NGDtuanh.Singleton;
using UnityEngine;

public class PlayerHUD : SceneSingleton<PlayerHUD> {
    [field: SerializeField] public StatsUI Stats { get; private set; }

    public DeadHUDHandler DeadHandler { get; private set; }
    public HealthBarUI HealthBar   { get; private set; }


    protected override void Awake() {
        base.Awake();

        DeadHandler = GetComponent<DeadHUDHandler>();
        HealthBar   = GetComponentInChildren<HealthBarUI>();
    }
}