using TMPro;
using Unity.NetCode;
using UnityEngine;

[RequireComponent(typeof(DisablableUIRoot))]
public class DeadHandler_Base : MonoBehaviour {
    [SerializeField] protected TextMeshProUGUI _DeadTimer;
    [SerializeField] protected NetCodeConfig   _NetConfig;

    protected DisablableUIRoot _ChildDeadStateTrigger;
    protected int              _TickRate;
    protected NetworkTick      _RespawnAtTick;
    protected int              _TotalWaitTick;
    protected float            _TotalWaitTime;

    protected virtual void Awake() {
        _ChildDeadStateTrigger = GetComponent<DisablableUIRoot>();
        _TickRate              = _NetConfig.ClientServerTickRate.SimulationTickRate;
    }

    public virtual void Dead(in NetworkTick deadAtTick, in NetworkTick respawnTick) {
        _ChildDeadStateTrigger.DisableAll();
        _DeadTimer.gameObject.SetActive(true);

        _RespawnAtTick = respawnTick;
        _TotalWaitTick = _RespawnAtTick.TicksSince(deadAtTick);
        _TotalWaitTime = (float)_TotalWaitTick / _TickRate;

        UpdateDead(deadAtTick);
    }

    public virtual void Respawn() {
        _ChildDeadStateTrigger.EnableAll();
        _DeadTimer.gameObject.SetActive(false);
    }

    public virtual void UpdateDead(in NetworkTick curTick) {
        _DeadTimer.text = ((int)(_TotalWaitTime * _RespawnAtTick.TicksSince(curTick) / _TotalWaitTick)).ToString();
    }
}