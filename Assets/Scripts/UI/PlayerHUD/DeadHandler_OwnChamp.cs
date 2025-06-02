using Unity.NetCode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DeadHandler_OwnChamp : DeadHandler_Base {
    private ClampedFloatParameter _Saturation;

    protected override void Awake() {
        base.Awake();

        if (true == FindAnyObjectByType<Volume>()?.profile.TryGet(out ColorAdjustments _ColorAdjust))
            _Saturation = _ColorAdjust.saturation;
        else Debug.LogError($"NGDtuanh: cannot get {nameof(ColorAdjustments)} from {nameof(Volume)}");
    }

    public override void Dead(in NetworkTick deadAtTick, in NetworkTick respawnTick) {
        base.Dead(deadAtTick, respawnTick);

        _Saturation.value = _Saturation.min;

        PlayerHUD.Instance.ActivableItems.StartDeadAllItems();
    }

    public override void Respawn() {
        base.Respawn();

        _Saturation.value = 0;

        PlayerHUD.Instance.ActivableItems.DoneDeadAllItems();
    }
}