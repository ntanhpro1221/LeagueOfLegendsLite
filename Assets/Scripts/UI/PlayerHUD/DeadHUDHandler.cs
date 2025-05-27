using TMPro;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(DisablableUIRoot))]
public class DeadHUDHandler : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _DeadTimer;
    [SerializeField] private NetCodeConfig   _NetConfig;
    [SerializeField] private Volume          _Volume;

    private ClampedFloatParameter _Saturation;
    private DisablableUIRoot      _HUDTrigger;
    private int                   _TickRate;
    private NetworkTick           _RespawnAtTick;
    private int                   _TotalWaitTick;
    private float                 _TotalWaitTime;

    private void Awake() {
        _HUDTrigger = GetComponent<DisablableUIRoot>();
        _TickRate   = _NetConfig.ClientServerTickRate.SimulationTickRate;
        if (!_Volume.profile.TryGet(out ColorAdjustments _ColorAdjust))
            Debug.LogError($"NGDtuanh: cannot get {nameof(ColorAdjustments)} from {nameof(Volume)}");
        _Saturation = _ColorAdjust.saturation;
    }

    public void Dead(in NetworkTick deadAtTick, in NetworkTick respawnTick) {
        _HUDTrigger.DisableAll();
        _DeadTimer.gameObject.SetActive(true);
        _Saturation.value = _Saturation.min;

        _RespawnAtTick = respawnTick;
        _TotalWaitTick = _RespawnAtTick.TicksSince(deadAtTick);
        _TotalWaitTime = (float)_TotalWaitTick / _TickRate;

        UpdateDead(deadAtTick);
    }

    public void Respawn() {
        _HUDTrigger.EnableAll();
        _DeadTimer.gameObject.SetActive(false);
        _Saturation.value = 0;
    }

    public void UpdateDead(in NetworkTick curTick) {
        _DeadTimer.text = ((int)(_TotalWaitTime * _RespawnAtTick.TicksSince(curTick) / _TotalWaitTick)).ToString();
    }
}