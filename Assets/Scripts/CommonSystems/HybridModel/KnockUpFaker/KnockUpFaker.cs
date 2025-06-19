using Unity.NetCode;
using UnityEngine;

public class KnockUpFaker : MonoBehaviour {
    private const float Epsilon = 0.01f;

    private Transform _Trans;
    private int       _TickRate;
    private float     _VelocityY;

    private void Awake() {
        _Trans    = transform;
        _TickRate = GameSO.TickRate;
    }

    public void PushKnockUp(in NetworkTick curTick, in NetworkTick endAtTick) {
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        float duration = (float)endAtTick.TicksSince(curTick) / _TickRate;
        _VelocityY = Mathf.Max(_VelocityY
          , -KnockUpFakerSettings.Gravity * duration / 2 - _Trans.localPosition.y / duration);
    }

    private void FixedUpdate() {
        var     prevPos = _Trans.localPosition;
        ref var prevY   = ref prevPos.y;
        if (prevY < Epsilon && _VelocityY < Epsilon) return;

        prevY                = Mathf.Max(0, prevY + _VelocityY * Time.fixedDeltaTime);
        _Trans.localPosition = prevPos;

        if (prevY < Epsilon)
            _VelocityY  =  0;
        else _VelocityY += KnockUpFakerSettings.Gravity * Time.fixedDeltaTime;
    }
}