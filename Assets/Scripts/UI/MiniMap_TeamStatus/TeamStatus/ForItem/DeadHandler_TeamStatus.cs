using Unity.NetCode;
using UnityEngine;

public class DeadHandler_TeamStatus : DeadHandler_Base {
    private GameObject _HealthBarUI;

    protected override void Awake() {
        base.Awake();

        _HealthBarUI = GetComponentInChildren<HealthBarUI>(true).gameObject;
    }

    public override void Dead(in NetworkTick deadAtTick, in NetworkTick respawnTick) {
        base.Dead(in deadAtTick, in respawnTick);
        
        _HealthBarUI.SetActive(false);
    }

    public override void Respawn() {
        base.Respawn();
        
        _HealthBarUI.SetActive(true);
    }
}