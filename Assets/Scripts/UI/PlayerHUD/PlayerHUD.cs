using NGDtuanh.Singleton;
using UnityEngine;

[RequireComponent(typeof(DeadHandler_OwnChamp))]
public class PlayerHUD : SceneSingleton<PlayerHUD> {
    [field: SerializeField] public StatsUI Stats { get; private set; }

    public DeadHandler_OwnChamp DeadHandler { get; private set; }
    public HealthBarUI          HealthBar   { get; private set; }


    protected override void Awake() {
        base.Awake();

        DeadHandler = GetComponent<DeadHandler_OwnChamp>();
        HealthBar   = GetComponentInChildren<HealthBarUI>();
    }
}